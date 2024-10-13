using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.Strings;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(UseExpressionResultSuggestion),
        typeof(UseAsCharacterSuggestion),
        typeof(UseStringListPatternSuggestion),
        typeof(UseOtherMethodSuggestion),
        typeof(RedundantArgumentSuggestion),
    ])]
public sealed class StringAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    [Pure]
    static bool IsStringComparison(IType type) => type.IsClrType(PredefinedType.STRING_COMPARISON_CLASS);

    [Pure]
    static string[]? TryGetParameterNamesIfMethodExists(string methodName, IClrTypeName[] parameterTypes, IPsiModule psiModule)
    {
        if (PredefinedType.STRING_FQN.TryGetTypeElement(psiModule) is { } stringType)
        {
            foreach (var method in stringType.Methods)
            {
                if (method is { TypeParameters: [] } && method.ShortName == methodName && method.Parameters.Count == parameterTypes.Length)
                {
                    if (parameterTypes is [])
                    {
                        return [];
                    }

                    var parameterNames = new string[parameterTypes.Length];

                    for (var i = 0; i < parameterNames.Length; i++)
                    {
                        if (parameterTypes[i].TryGetTypeElement(psiModule) is { } expectedType
                            && TypeEqualityComparer.Default.Equals(TypeFactory.CreateType(expectedType), method.Parameters[i].Type))
                        {
                            parameterNames[i] = method.Parameters[i].ShortName;
                        }
                    }

                    if (parameterNames.All(n => n is { }))
                    {
                        return parameterNames;
                    }
                }
            }
        }

        return null;
    }

    [Pure]
    static string? TryGetStringConstant(ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.String, StringValue: var value } } ? value : null;

    [Pure]
    static char? TryGetCharConstant(ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Char, CharValue: var value } } ? value : null;

    [Pure]
    static int? TryGetInt32Constant(ICSharpExpression? expression)
        => expression is IConstantValueOwner { ConstantValue: { Kind: ConstantValueKind.Int, IntValue: var value } } ? value : null;

    /// <remarks>
    /// <c>text.Contains("")</c> → <c>true</c><para/>
    /// <c>text.Contains(string)</c> → <c>text.Contains(char)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeContains_String(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        switch (TryGetStringConstant(valueArgument.Value))
        {
            case "" when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true", invocationExpression, "true"));
                break;

            case [var character] when TryGetParameterNamesIfMethodExists(
                nameof(string.Contains),
                [PredefinedType.CHAR_FQN],
                invocationExpression.PsiModule) is [var valueParameterName]:

                consumer.AddHighlighting(
                    new UseAsCharacterSuggestion(
                        "Use as character",
                        valueArgument,
                        valueArgument.NameIdentifier is { } ? valueParameterName : null,
                        character));
                break;
        }
    }

    /// <remarks>
    /// <c>text.Contains("", StringComparison)</c> → <c>true</c><para/>
    /// <c>text.Contains(string, StringComparison)</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeContains_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valueArgument)
    {
        switch (TryGetStringConstant(valueArgument.Value))
        {
            case "" when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true", invocationExpression, "true"));
                break;

            case [var character] when TryGetParameterNamesIfMethodExists(
                nameof(string.Contains),
                [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                invocationExpression.PsiModule) is [var valueParameterName, _]:

                consumer.AddHighlighting(
                    new UseAsCharacterSuggestion(
                        "Use as character",
                        valueArgument,
                        valueArgument.NameIdentifier is { } ? valueParameterName : null,
                        character));
                break;
        }
    }

    /// <remarks>
    /// <c>text.EndsWith(char)</c> → <c>text is [.., var lastCharacter] &amp;&amp; lastCharacter == value</c> (C# 11)<para/>
    /// <c>text.EndsWith(char)</c> → <c>text is [.., value]</c> (C# 11)
    /// </remarks>
    static void AnalyzeEndsWith_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
            && !invocationExpression.IsUsedAsStatement()
            && valueArgument.Value is { })
        {
            if (TryGetCharConstant(valueArgument.Value) is { } character)
            {
                consumer.AddHighlighting(
                    new UseStringListPatternSuggestion(
                        "Use list pattern",
                        invocationExpression,
                        invokedExpression,
                        ListPatternSuggestionKind.LastCharacter,
                        [character]));
            }
            else
            {
                consumer.AddHighlighting(
                    new UseStringListPatternSuggestion(
                        "Use list pattern",
                        invocationExpression,
                        invokedExpression,
                        ListPatternSuggestionKind.LastCharacter,
                        valueArgument.Value.GetText()));
            }
        }
    }

    /// <remarks>
    /// <c>text.EndsWith("")</c> → <c>true</c>
    /// </remarks>
    static void AnalyzeEndsWith_String(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && TryGetStringConstant(valueArgument.Value) == "")
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true", invocationExpression, "true"));
        }
    }

    /// <remarks>
    /// <c>text.EndsWith("", StringComparison)</c> → <c>true</c><para/>
    /// <c>text.EndsWith(string, Ordinal)</c> → <c>text is [.., c]</c> (C# 11)<para/>
    /// <c>text.EndsWith(string, OrdinalIgnoresCase)</c> → <c>text is [.., l or u]</c> (C# 11)
    /// </remarks>
    static void AnalyzeEndsWith_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && TryGetStringConstant(valueArgument.Value) == "")
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true", invocationExpression, "true"));
        }

        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
            && !invocationExpression.IsUsedAsStatement()
            && TryGetStringConstant(valueArgument.Value) is [var character]
            && comparisonTypeArgument.Value is IReferenceExpression { Reference: var reference }
            && reference.Resolve() is { DeclaredElement: IField { ContainingType: var enumType, ShortName: var enumMemberName } }
            && enumType.IsClrType(PredefinedType.STRING_COMPARISON_CLASS))
        {
            switch (enumMemberName)
            {
                case nameof(StringComparison.Ordinal):
                    consumer.AddHighlighting(
                        new UseStringListPatternSuggestion(
                            "Use list pattern",
                            invocationExpression,
                            invokedExpression,
                            ListPatternSuggestionKind.LastCharacter,
                            [character]));
                    break;

                case nameof(StringComparison.OrdinalIgnoreCase):
                    var lowerCaseCharacter = char.ToLowerInvariant(character);
                    var upperCaseCharacter = char.ToUpperInvariant(character);

                    if (lowerCaseCharacter == upperCaseCharacter)
                    {
                        consumer.AddHighlighting(
                            new UseStringListPatternSuggestion(
                                "Use list pattern",
                                invocationExpression,
                                invokedExpression,
                                ListPatternSuggestionKind.LastCharacter,
                                [lowerCaseCharacter]));
                    }
                    else
                    {
                        consumer.AddHighlighting(
                            new UseStringListPatternSuggestion(
                                "Use list pattern",
                                invocationExpression,
                                invokedExpression,
                                ListPatternSuggestionKind.LastCharacter,
                                [lowerCaseCharacter, upperCaseCharacter]));
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(char) == 0</c> → <c>text is [var firstCharacter, ..] &amp;&amp; firstCharacter == value</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) == 0</c> → <c>text is [value, ..]</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) != 0</c> → <c>text is not [var firstCharacter, ..] || firstCharacter != value</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) != 0</c> → <c>text is not [value, ..]</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) > -1</c> → <c>text.Contains(char)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char) != -1</c> → <c>text.Contains(char) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(char) >= 0</c> → <c>text.Contains(char) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(char) == -1</c> → <c>!text.Contains(char) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(char) &lt; 0</c> → <c>!text.Contains(char) (.NET Core 2.1)</c>
    /// </remarks>
    static void AnalyzeIndexOf_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        switch (invocationExpression.Parent)
        {
            case IEqualityExpression equalityExpression when equalityExpression.LeftOperand == invocationExpression && valueArgument.Value is { }:
                switch (equalityExpression.EqualityType, TryGetInt32Constant(equalityExpression.RightOperand))
                {
                    case (EqualityExpressionType.EQEQ, 0):
                        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110)
                        {
                            if (TryGetCharConstant(valueArgument.Value) is { } character)
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.FirstCharacter,
                                        [character],
                                        equalityExpression));
                            }
                            else
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.FirstCharacter,
                                        valueArgument.Value.GetText(),
                                        equalityExpression));
                            }
                        }
                        break;

                    case (EqualityExpressionType.NE, 0):
                        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110)
                        {
                            if (TryGetCharConstant(valueArgument.Value) is { } character)
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.NotFirstCharacter,
                                        [character],
                                        equalityExpression));
                            }
                            else
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.NotFirstCharacter,
                                        valueArgument.Value.GetText(),
                                        equalityExpression));
                            }
                        }
                        break;

                    case (EqualityExpressionType.NE, -1):
                        if (TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.CHAR_FQN],
                                invocationExpression.PsiModule) is [_])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    false,
                                    [valueArgument.Value.GetText()],
                                    equalityExpression));
                        }
                        break;

                    case (EqualityExpressionType.EQEQ, -1):
                        if (TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.CHAR_FQN],
                                invocationExpression.PsiModule) is [_])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    true,
                                    [valueArgument.Value.GetText()],
                                    equalityExpression));
                        }
                        break;
                }
                break;

            case IRelationalExpression relationalExpression when relationalExpression.LeftOperand == invocationExpression
                && TryGetInt32Constant(relationalExpression.RightOperand) is { } value
                && valueArgument.Value is { }:

                var tokenType = relationalExpression.OperatorSign.GetTokenType();

                if ((tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                    && TryGetParameterNamesIfMethodExists(nameof(string.Contains), [PredefinedType.CHAR_FQN], invocationExpression.PsiModule) is [_])
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(string.Contains)}' method",
                            invocationExpression,
                            invokedExpression,
                            nameof(string.Contains),
                            false,
                            [valueArgument.Value.GetText()],
                            relationalExpression));
                }

                if (tokenType == CSharpTokenType.LT
                    && value == 0
                    && TryGetParameterNamesIfMethodExists(nameof(string.Contains), [PredefinedType.CHAR_FQN], invocationExpression.PsiModule) is [_])
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(string.Contains)}' method",
                            invocationExpression,
                            invokedExpression,
                            nameof(string.Contains),
                            true,
                            [valueArgument.Value.GetText()],
                            relationalExpression));
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(char, 0)</c> → <c>text.IndexOf(char)</c>
    /// </remarks>
    static void AnalyzeIndexOf_Char_Int32(IHighlightingConsumer consumer, ICSharpArgument startIndexArgument)
    {
        if (TryGetInt32Constant(startIndexArgument.Value) == 0)
        {
            consumer.AddHighlighting(new RedundantArgumentSuggestion("Argument 0 is redundant", startIndexArgument));
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(char, StringComparison) > -1</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) != -1</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) >= 0</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) == -1</c> → <c>!text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) &lt; 0</c> → <c>!text.Contains(char, StringComparison)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeIndexOf_Char_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        switch (invocationExpression.Parent)
        {
            case IEqualityExpression equalityExpression when equalityExpression.LeftOperand == invocationExpression
                && valueArgument.Value is { }
                && comparisonTypeArgument.Value is { }:

                switch (equalityExpression.EqualityType, TryGetInt32Constant(equalityExpression.RightOperand))
                {
                    case (EqualityExpressionType.NE, -1):
                        if (TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                invocationExpression.PsiModule) is [_, _])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    false,
                                    [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                    equalityExpression));
                        }
                        break;

                    case (EqualityExpressionType.EQEQ, -1):
                        if (TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                invocationExpression.PsiModule) is [_, _])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    true,
                                    [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                    equalityExpression));
                        }
                        break;
                }
                break;

            case IRelationalExpression relationalExpression when relationalExpression.LeftOperand == invocationExpression
                && TryGetInt32Constant(relationalExpression.RightOperand) is { } value
                && valueArgument.Value is { }
                && comparisonTypeArgument.Value is { }:

                var tokenType = relationalExpression.OperatorSign.GetTokenType();

                if ((tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                    && TryGetParameterNamesIfMethodExists(
                        nameof(string.Contains),
                        [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                        invocationExpression.PsiModule) is [_, _])
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(string.Contains)}' method",
                            invocationExpression,
                            invokedExpression,
                            nameof(string.Contains),
                            false,
                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                            relationalExpression));
                }

                if (tokenType == CSharpTokenType.LT
                    && value == 0
                    && TryGetParameterNamesIfMethodExists(
                        nameof(string.Contains),
                        [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                        invocationExpression.PsiModule) is [_, _])
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(string.Contains)}' method",
                            invocationExpression,
                            invokedExpression,
                            nameof(string.Contains),
                            true,
                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                            relationalExpression));
                }

                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOf("")</c> → 0<para/>
    /// <c>text.IndexOf(string)</c> → <c>text.IndexOf(char, CurrentCulture)</c><para/>
    /// <c>text.IndexOf(string) == 0</c> → <c>text.StartsWith(string)</c><para/>
    /// <c>text.IndexOf(string) != 0</c> → <c>!text.StartsWith(string)</c><para/>
    /// <c>text.IndexOf(string) > -1</c> → <c>text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) != -1</c> → <c>text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) >= 0</c> → <c>text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) == -1</c> → <c>!text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) &lt; 0</c> → <c>!text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeIndexOf_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        switch (TryGetStringConstant(valueArgument.Value))
        {
            case "" when !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always 0", invocationExpression, "0"));
                break;

            case [var character] when TryGetParameterNamesIfMethodExists(
                nameof(string.IndexOf),
                [PredefinedType.CHAR_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                invocationExpression.PsiModule) is [var valueParameterName, _]:
                consumer.AddHighlighting(
                    new UseAsCharacterSuggestion(
                        "Use as character",
                        valueArgument,
                        valueArgument.NameIdentifier is { } ? valueParameterName : null,
                        character,
                        $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"));
                break;

            default:
                switch (invocationExpression.Parent)
                {
                    case IEqualityExpression equalityExpression
                        when equalityExpression.LeftOperand == invocationExpression && valueArgument.Value is { }:

                        switch (equalityExpression.EqualityType, TryGetInt32Constant(equalityExpression.RightOperand))
                        {
                            case (EqualityExpressionType.EQEQ, 0):
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(string.StartsWith)}' method",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(string.StartsWith),
                                        false,
                                        [valueArgument.Value.GetText()],
                                        equalityExpression));
                                break;

                            case (EqualityExpressionType.NE, 0):
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(string.StartsWith)}' method",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(string.StartsWith),
                                        true,
                                        [valueArgument.Value.GetText()],
                                        equalityExpression));
                                break;

                            case (EqualityExpressionType.NE, -1):
                                if (TryGetParameterNamesIfMethodExists(
                                        nameof(string.Contains),
                                        [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                        invocationExpression.PsiModule) is [_, _])
                                {
                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.Contains)}' method",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.Contains),
                                            false,
                                            [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                            equalityExpression));
                                }
                                break;

                            case (EqualityExpressionType.EQEQ, -1):
                                if (TryGetParameterNamesIfMethodExists(
                                        nameof(string.Contains),
                                        [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                        invocationExpression.PsiModule) is [_, _])
                                {
                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.Contains)}' method",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.Contains),
                                            true,
                                            [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                            equalityExpression));
                                }
                                break;
                        }
                        break;

                    case IRelationalExpression relationalExpression when relationalExpression.LeftOperand == invocationExpression
                        && TryGetInt32Constant(relationalExpression.RightOperand) is { } value
                        && valueArgument.Value is { }:

                        var tokenType = relationalExpression.OperatorSign.GetTokenType();

                        if ((tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                            && TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                invocationExpression.PsiModule) is [_, _])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    false,
                                    [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                    relationalExpression));
                        }

                        if (tokenType == CSharpTokenType.LT
                            && value == 0
                            && TryGetParameterNamesIfMethodExists(
                                nameof(string.Contains),
                                [PredefinedType.STRING_FQN, PredefinedType.STRING_COMPARISON_CLASS],
                                invocationExpression.PsiModule) is [_, _])
                        {
                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    true,
                                    [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                    relationalExpression));
                        }

                        break;
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(string, 0)</c> → <c>text.IndexOf(string)</c>
    /// </remarks>
    static void AnalyzeIndexOf_String_Int32(IHighlightingConsumer consumer, ICSharpArgument startIndexArgument)
    {
        if (TryGetInt32Constant(startIndexArgument.Value) == 0)
        {
            consumer.AddHighlighting(new RedundantArgumentSuggestion("Argument 0 is redundant", startIndexArgument));
        }
    }

    /// <remarks>
    /// <c>text.IndexOf("", StringComparison)</c> → <c>0</c><para/>
    /// <c>text.IndexOf(string, StringComparison)</c> → <c>text.IndexOf(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string, StringComparison) == 0</c> → <c>text.StartsWith(string, StringComparison)</c><para/>
    /// <c>text.IndexOf(string, StringComparison) != 0</c> → <c>!text.StartsWith(string, StringComparison)</c><para/>
    /// <c>text.IndexOf(string, StringComparison) > -1</c> → <c>text.Contains(string, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string, StringComparison) != -1</c> → <c>text.Contains(string, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string, StringComparison) >= 0</c> → <c>text.Contains(string, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string, StringComparison) == -1</c> → <c>!text.Contains(string, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string, StringComparison) &lt; 0</c> → <c>!text.Contains(string, StringComparison)</c> (.NET Core 2.1)
    /// </remarks>
    static void AnalyzeIndexOf_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument) { }

    /// <remarks>
    /// <c>text.IndexOf(string, 0, StringComparison)</c> → <c>text.IndexOf(string, StringComparison)</c>
    /// </remarks>
    static void AnalyzeIndexOf_String_Int32_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument startIndexArgument,
        ICSharpArgument comparisonTypeArgument) { }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { QualifierExpression: { }, Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod method
            && method.ContainingType.IsSystemString()
            && method is { IsStatic: false, AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC })
        {
            switch (method)
            {
                // Contains
                case { ShortName: nameof(string.Contains), TypeParameters: [], Parameters: [{ Type: var valueType }] }
                    when valueType.IsString() && element.Arguments is [var valueArgument]:
                    AnalyzeContains_String(consumer, element, valueArgument);
                    break;

                case
                {
                    ShortName: nameof(string.Contains),
                    TypeParameters: [],
                    Parameters: [{ Type: var valueType }, { Type: var stringComparisonType }],
                } when valueType.IsString() && IsStringComparison(stringComparisonType) && element.Arguments is [var valueArgument, _]:
                    AnalyzeContains_String_StringComparison(consumer, element, valueArgument);
                    break;

                // EndsWith
                case { ShortName: nameof(string.EndsWith), TypeParameters: [], Parameters: [{ Type: var valueType }] }
                    when valueType.IsChar() && element.Arguments is [var valueArgument]:
                    AnalyzeEndsWith_Char(consumer, element, invokedExpression, valueArgument);
                    break;

                case { ShortName: nameof(string.EndsWith), TypeParameters: [], Parameters: [{ Type: var valueType }] }
                    when valueType.IsString() && element.Arguments is [var valueArgument]:
                    AnalyzeEndsWith_String(consumer, element, valueArgument);
                    break;

                case
                    {
                        ShortName: nameof(string.EndsWith),
                        TypeParameters: [],
                        Parameters: [{ Type: var valueType }, { Type: var stringComparisonType }],
                    } when valueType.IsString()
                    && IsStringComparison(stringComparisonType)
                    && element.Arguments is [var valueArgument, var comparisonTypeArgument]:
                    AnalyzeEndsWith_String_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                    break;

                // IndexOf
                case { ShortName: nameof(string.IndexOf), TypeParameters: [], Parameters: [{ Type: var valueType }] }
                    when valueType.IsChar() && element.Arguments is [var valueArgument]:
                    AnalyzeIndexOf_Char(consumer, element, invokedExpression, valueArgument);
                    break;

                case { ShortName: nameof(string.IndexOf), TypeParameters: [], Parameters: [{ Type: var valueType }, { Type: var startIndexType }] }
                    when valueType.IsChar() && startIndexType.IsInt() && element.Arguments is [_, var startIndexArgument]:
                    AnalyzeIndexOf_Char_Int32(consumer, startIndexArgument);
                    break;

                case
                    {
                        ShortName: nameof(string.IndexOf),
                        TypeParameters: [],
                        Parameters: [{ Type: var valueType }, { Type: var stringComparisonType }],
                    } when valueType.IsChar()
                    && IsStringComparison(stringComparisonType)
                    && element.Arguments is [var valueArgument, var comparisonTypeArgument]:
                    AnalyzeIndexOf_Char_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                    break;

                case { ShortName: nameof(string.IndexOf), TypeParameters: [], Parameters: [{ Type: var valueType }] }
                    when valueType.IsString() && element.Arguments is [var valueArgument]:
                    AnalyzeIndexOf_String(consumer, element, invokedExpression, valueArgument);
                    break;

                case { ShortName: nameof(string.IndexOf), TypeParameters: [], Parameters: [{ Type: var valueType }, { Type: var startIndexType }] }
                    when valueType.IsString() && startIndexType.IsInt() && element.Arguments is [_, var startIndexArgument]:
                    AnalyzeIndexOf_String_Int32(consumer, startIndexArgument);
                    break;

                case
                    {
                        ShortName: nameof(string.IndexOf),
                        TypeParameters: [],
                        Parameters: [{ Type: var valueType }, { Type: var stringComparisonType }],
                    } when valueType.IsString()
                    && IsStringComparison(stringComparisonType)
                    && element.Arguments is [var valueArgument, var comparisonTypeArgument]:
                    AnalyzeIndexOf_String_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                    break;

                case
                    {
                        ShortName: nameof(string.IndexOf),
                        TypeParameters: [],
                        Parameters: [{ Type: var valueType }, { Type: var startIndexType }, { Type: var stringComparisonType }],
                    } when valueType.IsString()
                    && startIndexType.IsInt()
                    && IsStringComparison(stringComparisonType)
                    && element.Arguments is [var valueArgument, var startIndexArgument, var comparisonTypeArgument]:
                    AnalyzeIndexOf_String_Int32_StringComparison(
                        consumer,
                        element,
                        invokedExpression,
                        valueArgument,
                        startIndexArgument,
                        comparisonTypeArgument);
                    break;
            }
        }
    }
}