using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Analyzers.Strings.Collections;
using ReCommendedExtension.Analyzers.Strings.MethodFinding;

namespace ReCommendedExtension.Analyzers.Strings;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(UseExpressionResultSuggestion),
        typeof(PassSingleCharacterSuggestion),
        typeof(PassSingleCharactersSuggestion),
        typeof(UseStringListPatternSuggestion),
        typeof(UseOtherMethodSuggestion),
        typeof(RedundantArgumentHint),
        typeof(RedundantElementHint),
        typeof(UseStringPropertySuggestion),
        typeof(RedundantMethodInvocationHint),
        typeof(UseRangeIndexerSuggestion),
    ])]
public sealed class StringAnalyzer(NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
    : ElementProblemAnalyzer<IInvocationExpression>
{
    [Pure]
    static bool IsStringComparison(IType type) => type.IsClrType(PredefinedType.STRING_COMPARISON_CLASS);

    [Pure]
    static bool IsStringSplitOptions(IType type) => type.IsClrType(ClrTypeNames.StringSplitOptions);

    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class ParameterTypes
    {
        public static IReadOnlyList<ParameterType> Char { get; } = [new() { ClrTypeName = PredefinedType.CHAR_FQN }];

        public static IReadOnlyList<ParameterType> CharArray { get; } = [new ArrayParameterType { ClrTypeName = PredefinedType.CHAR_FQN }];

        public static IReadOnlyList<ParameterType> Char_Char { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN }, new() { ClrTypeName = PredefinedType.CHAR_FQN },
        ];

        public static IReadOnlyList<ParameterType> Char_Int32 { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN }, new() { ClrTypeName = PredefinedType.INT_FQN },
        ];

        public static IReadOnlyList<ParameterType> Char_StringComparison { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN }, new() { ClrTypeName = PredefinedType.STRING_COMPARISON_CLASS },
        ];

        public static IReadOnlyList<ParameterType> Char_StringSplitOptions { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN }, new() { ClrTypeName = ClrTypeNames.StringSplitOptions },
        ];

        public static IReadOnlyList<ParameterType> Char_Int32_Int32 { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN },
            new() { ClrTypeName = PredefinedType.INT_FQN },
            new() { ClrTypeName = PredefinedType.INT_FQN },
        ];

        public static IReadOnlyList<ParameterType> Char_Int32_StringSplitOptions { get; } =
        [
            new() { ClrTypeName = PredefinedType.CHAR_FQN },
            new() { ClrTypeName = PredefinedType.INT_FQN },
            new() { ClrTypeName = ClrTypeNames.StringSplitOptions },
        ];

        public static IReadOnlyList<ParameterType> CharArray_StringSplitOptions { get; } =
        [
            new ArrayParameterType { ClrTypeName = PredefinedType.CHAR_FQN }, new() { ClrTypeName = ClrTypeNames.StringSplitOptions },
        ];

        public static IReadOnlyList<ParameterType> CharArray_Int32_StringSplitOptions { get; } =
        [
            new ArrayParameterType { ClrTypeName = PredefinedType.CHAR_FQN },
            new() { ClrTypeName = PredefinedType.INT_FQN },
            new() { ClrTypeName = ClrTypeNames.StringSplitOptions },
        ];

        public static IReadOnlyList<ParameterType> String { get; } = [new() { ClrTypeName = PredefinedType.STRING_FQN }];

        public static IReadOnlyList<ParameterType> String_StringComparison { get; } =
        [
            new() { ClrTypeName = PredefinedType.STRING_FQN }, new() { ClrTypeName = PredefinedType.STRING_COMPARISON_CLASS },
        ];

        public static IReadOnlyList<ParameterType> Int32 { get; } = [new() { ClrTypeName = PredefinedType.INT_FQN }];
    }

    [Pure]
    static bool ArrayEmptyMethodExists(IPsiModule psiModule)
        => PredefinedType.ARRAY_FQN.TryGetTypeElement(psiModule) is { } arrayType
            && arrayType.Methods.Any(
                method => method is
                {
                    ShortName: nameof(Array.Empty),
                    IsStatic: true,
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                    TypeParameters: [_],
                    Parameters: [],
                });

    [Pure]
    static string CreateStringArray(string[] items, ICSharpExpression context)
    {
        if (context.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp120 && context.TryGetTargetType() is { })
        {
            return $"[{string.Join(", ", items)}]";
        }

        if (items is [])
        {
            if (ArrayEmptyMethodExists(context.GetPsiModule()))
            {
                return $"{nameof(Array)}.{nameof(Array.Empty)}<string>()";
            }

            return "new string[0]";
        }

        return $$"""new[] { {{string.Join(", ", items)}} }""";
    }

    /// <remarks>
    /// <c>text.Contains("")</c> → <c>true</c><para/>
    /// <c>text.Contains(string)</c> → <c>text.Contains(char)</c> (.NET Core 2.1)
    /// </remarks>
    void AnalyzeContains_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (valueArgument.Value.TryGetStringConstant())
        {
            case "" when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
                break;

            case [var character] when PredefinedType.STRING_FQN.HasMethod(
                nameof(string.Contains),
                ParameterTypes.Char,
                valueArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        parameterNames is [var valueParameterName] ? valueParameterName : null,
                        character));
                break;
        }
    }

    /// <remarks>
    /// <c>text.Contains("", StringComparison)</c> → <c>true</c><para/>
    /// <c>text.Contains(string, StringComparison)</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)
    /// </remarks>
    void AnalyzeContains_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (valueArgument.Value.TryGetStringConstant())
        {
            case "" when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
                break;

            case [var character] when PredefinedType.STRING_FQN.HasMethod(
                nameof(string.Contains),
                ParameterTypes.Char_StringComparison,
                valueArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        parameterNames is [var valueParameterName, _] ? valueParameterName : null,
                        character));
                break;
        }
    }

    /// <remarks>
    /// <c>text.EndsWith(char)</c> → <c>text is [.., var lastChar] &amp;&amp; lastChar == value</c> (C# 11)<para/>
    /// <c>text.EndsWith(char)</c> → <c>text is [.., value]</c> (C# 11)
    /// </remarks>
    void AnalyzeEndsWith_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
            && valueArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseStringListPatternSuggestion(
                    "Use list pattern.",
                    invocationExpression,
                    invokedExpression,
                    ListPatternSuggestionKind.LastCharacter,
                    [(valueArgument.Value.GetText(), valueArgument.Value.TryGetCharConstant() is { })]));
        }
    }

    /// <remarks>
    /// <c>text.EndsWith("")</c> → <c>true</c>
    /// </remarks>
    void AnalyzeEndsWith_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
            && valueArgument.Value.TryGetStringConstant() == "")
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
        }
    }

    /// <remarks>
    /// <c>text.EndsWith("", StringComparison)</c> → <c>true</c><para/>
    /// <c>text.EndsWith(string, Ordinal)</c> → <c>text is [.., c]</c> (C# 11)<para/>
    /// <c>text.EndsWith(string, OrdinalIgnoresCase)</c> → <c>text is [.., l or u]</c> (C# 11)
    /// </remarks>
    void AnalyzeEndsWith_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            switch (valueArgument.Value.TryGetStringConstant())
            {
                case "":
                    consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
                    break;

                case [var character] when invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110:
                    switch (comparisonTypeArgument.Value.TryGetStringComparisonConstant())
                    {
                        case StringComparison.Ordinal:
                            consumer.AddHighlighting(
                                new UseStringListPatternSuggestion(
                                    "Use list pattern.",
                                    invocationExpression,
                                    invokedExpression,
                                    ListPatternSuggestionKind.LastCharacter,
                                    [($"'{character}'", true)]));
                            break;

                        case StringComparison.OrdinalIgnoreCase:
                            var lowerCaseCharacter = char.ToLowerInvariant(character);
                            var upperCaseCharacter = char.ToUpperInvariant(character);

                            if (lowerCaseCharacter == upperCaseCharacter)
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern.",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.LastCharacter,
                                        [($"'{lowerCaseCharacter}'", true)]));
                            }
                            else
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern.",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.LastCharacter,
                                        [($"'{lowerCaseCharacter}'", true), ($"'{upperCaseCharacter}'", true)]));
                            }
                            break;
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(char) == 0</c> → <c>text is [var firstChar, ..] &amp;&amp; firstChar == value</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) == 0</c> → <c>text is [value, ..]</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) != 0</c> → <c>text is not [var firstChar, ..] || firstChar != value</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) != 0</c> → <c>text is not [value, ..]</c> (C# 11)<para/>
    /// <c>text.IndexOf(char) > -1</c> → <c>text.Contains(char)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char) != -1</c> → <c>text.Contains(char) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(char) >= 0</c> → <c>text.Contains(char) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(char) == -1</c> → <c>!text.Contains(char) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(char) &lt; 0</c> → <c>!text.Contains(char) (.NET Core 2.1)</c>
    /// </remarks>
    void AnalyzeIndexOf_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (invocationExpression.Parent)
        {
            case IEqualityExpression equalityExpression when equalityExpression.LeftOperand == invocationExpression && valueArgument.Value is { }:
                switch (equalityExpression.EqualityType, equalityExpression.RightOperand.TryGetInt32Constant())
                {
                    case (EqualityExpressionType.EQEQ, 0) when invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110:
                        consumer.AddHighlighting(
                            new UseStringListPatternSuggestion(
                                "Use list pattern.",
                                invocationExpression,
                                invokedExpression,
                                ListPatternSuggestionKind.FirstCharacter,
                                [(valueArgument.Value.GetText(), valueArgument.Value.TryGetCharConstant() is { })],
                                equalityExpression));
                        break;

                    case (EqualityExpressionType.NE, 0) when invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110:
                        consumer.AddHighlighting(
                            new UseStringListPatternSuggestion(
                                "Use list pattern.",
                                invocationExpression,
                                invokedExpression,
                                ListPatternSuggestionKind.NotFirstCharacter,
                                [(valueArgument.Value.GetText(), valueArgument.Value.TryGetCharConstant() is { })],
                                equalityExpression));
                        break;

                    case (EqualityExpressionType.NE, -1)
                        when invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
                        && PredefinedType.STRING_FQN.HasMethod(nameof(string.Contains), ParameterTypes.Char, invocationExpression.PsiModule):

                        consumer.AddHighlighting(
                            new UseOtherMethodSuggestion(
                                $"Use the '{nameof(string.Contains)}' method.",
                                invocationExpression,
                                invokedExpression,
                                nameof(string.Contains),
                                false,
                                [valueArgument.Value.GetText()],
                                equalityExpression));
                        break;

                    case (EqualityExpressionType.EQEQ, -1)
                        when invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
                        && PredefinedType.STRING_FQN.HasMethod(nameof(string.Contains), ParameterTypes.Char, invocationExpression.PsiModule):

                        consumer.AddHighlighting(
                            new UseOtherMethodSuggestion(
                                $"Use the '{nameof(string.Contains)}' method.",
                                invocationExpression,
                                invokedExpression,
                                nameof(string.Contains),
                                true,
                                [valueArgument.Value.GetText()],
                                equalityExpression));
                        break;
                }
                break;

            case IRelationalExpression relationalExpression when relationalExpression.LeftOperand == invocationExpression
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
                && relationalExpression.RightOperand.TryGetInt32Constant() is { } value
                && valueArgument.Value is { }
                && PredefinedType.STRING_FQN.HasMethod(nameof(string.Contains), ParameterTypes.Char, invocationExpression.PsiModule):

                var tokenType = relationalExpression.OperatorSign.GetTokenType();

                if (tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(string.Contains)}' method.",
                            invocationExpression,
                            invokedExpression,
                            nameof(string.Contains),
                            false,
                            [valueArgument.Value.GetText()],
                            relationalExpression));
                }

                if (tokenType == CSharpTokenType.LT && value == 0)
                {
                    consumer.AddHighlighting(
                        new UseOtherMethodSuggestion(
                            $"Use the '{nameof(string.Contains)}' method.",
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
    static void AnalyzeIndexOf_Char_Int32(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument startIndexArgument)
    {
        if (startIndexArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.STRING_FQN.HasMethod(nameof(string.IndexOf), ParameterTypes.Char, invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", startIndexArgument));
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(char, StringComparison) > -1</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) != -1</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) >= 0</c> → <c>text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) == -1</c> → <c>!text.Contains(char, StringComparison)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(char, StringComparison) &lt; 0</c> → <c>!text.Contains(char, StringComparison)</c> (.NET Core 2.1)
    /// </remarks>
    void AnalyzeIndexOf_Char_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (PredefinedType.STRING_FQN.HasMethod(nameof(string.Contains), ParameterTypes.Char_StringComparison, invocationExpression.PsiModule))
        {
            switch (invocationExpression.Parent)
            {
                case IEqualityExpression equalityExpression when equalityExpression.LeftOperand == invocationExpression
                    && valueArgument.Value is { }
                    && comparisonTypeArgument.Value is { }:

                    switch (equalityExpression.EqualityType, equalityExpression.RightOperand.TryGetInt32Constant())
                    {
                        case (EqualityExpressionType.NE, -1)
                            when invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                            consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(string.Contains)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(string.Contains),
                                        false,
                                        [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                        equalityExpression));
                            break;

                        case (EqualityExpressionType.EQEQ, -1)
                            when invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                            consumer.AddHighlighting(
                                new UseOtherMethodSuggestion(
                                    $"Use the '{nameof(string.Contains)}' method.",
                                    invocationExpression,
                                    invokedExpression,
                                    nameof(string.Contains),
                                    true,
                                    [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                    equalityExpression));
                            break;
                    }
                    break;

                case IRelationalExpression relationalExpression when relationalExpression.LeftOperand == invocationExpression
                    && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
                    && relationalExpression.RightOperand.TryGetInt32Constant() is { } value
                    && valueArgument.Value is { }
                    && comparisonTypeArgument.Value is { }:

                    var tokenType = relationalExpression.OperatorSign.GetTokenType();

                    if (tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                    {
                        consumer.AddHighlighting(
                            new UseOtherMethodSuggestion(
                                $"Use the '{nameof(string.Contains)}' method.",
                                invocationExpression,
                                invokedExpression,
                                nameof(string.Contains),
                                false,
                                [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                relationalExpression));
                    }

                    if (tokenType == CSharpTokenType.LT && value == 0)
                    {
                        consumer.AddHighlighting(
                            new UseOtherMethodSuggestion(
                                $"Use the '{nameof(string.Contains)}' method.",
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
    }

    /// <remarks>
    /// <c>text.IndexOf("")</c> → 0<para/>
    /// <c>text.IndexOf(string)</c> → <c>text.IndexOf(char, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) == 0</c> → <c>text.StartsWith(string)</c><para/>
    /// <c>text.IndexOf(string) != 0</c> → <c>!text.StartsWith(string)</c><para/>
    /// <c>text.IndexOf(string) > -1</c> → <c>text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) != -1</c> → <c>text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) >= 0</c> → <c>text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) == -1</c> → <c>!text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(string) &lt; 0</c> → <c>!text.Contains(string, CurrentCulture)</c> (.NET Core 2.1)
    /// </remarks>
    void AnalyzeIndexOf_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (valueArgument.Value.TryGetStringConstant())
        {
            case "" when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always 0.", invocationExpression, "0"));
                break;

            case [var character] when PredefinedType.STRING_FQN.HasMethod(
                nameof(string.IndexOf),
                ParameterTypes.Char_StringComparison,
                valueArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        parameterNames is [var valueParameterName, _] ? valueParameterName : null,
                        character,
                        $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"));
                break;

            default:
                if (invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
                {
                    switch (invocationExpression.Parent)
                    {
                        case IEqualityExpression equalityExpression
                            when equalityExpression.LeftOperand == invocationExpression && valueArgument.Value is { }:

                            switch (equalityExpression.EqualityType, equalityExpression.RightOperand.TryGetInt32Constant())
                            {
                                case (EqualityExpressionType.EQEQ, 0) when PredefinedType.STRING_FQN.HasMethod(
                                    nameof(string.StartsWith),
                                    ParameterTypes.String,
                                    invocationExpression.PsiModule):

                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.StartsWith)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.StartsWith),
                                            false,
                                            [valueArgument.Value.GetText()],
                                            equalityExpression));
                                    break;

                                case (EqualityExpressionType.NE, 0) when PredefinedType.STRING_FQN.HasMethod(
                                    nameof(string.StartsWith),
                                    ParameterTypes.String,
                                    invocationExpression.PsiModule):

                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.StartsWith)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.StartsWith),
                                            true,
                                            [valueArgument.Value.GetText()],
                                            equalityExpression));
                                    break;

                                case (EqualityExpressionType.NE, -1) when PredefinedType.STRING_FQN.HasMethod(
                                    nameof(string.Contains),
                                    ParameterTypes.String_StringComparison,
                                    invocationExpression.PsiModule):

                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.Contains)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.Contains),
                                            false,
                                            [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                            equalityExpression));
                                    break;

                                case (EqualityExpressionType.EQEQ, -1) when PredefinedType.STRING_FQN.HasMethod(
                                    nameof(string.Contains),
                                    ParameterTypes.String_StringComparison,
                                    invocationExpression.PsiModule):

                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.Contains)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.Contains),
                                            true,
                                            [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                            equalityExpression));
                                    break;
                            }
                            break;

                        case IRelationalExpression relationalExpression when relationalExpression.LeftOperand == invocationExpression
                            && relationalExpression.RightOperand.TryGetInt32Constant() is { } value
                            && valueArgument.Value is { }
                            && PredefinedType.STRING_FQN.HasMethod(
                                nameof(string.Contains),
                                ParameterTypes.String_StringComparison,
                                invocationExpression.PsiModule):

                            var tokenType = relationalExpression.OperatorSign.GetTokenType();

                            if (tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                            {
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(string.Contains)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(string.Contains),
                                        false,
                                        [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                        relationalExpression));
                            }

                            if (tokenType == CSharpTokenType.LT && value == 0)
                            {
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(string.Contains)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(string.Contains),
                                        true,
                                        [valueArgument.Value.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"],
                                        relationalExpression));
                            }

                            break;
                    }
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(string, 0)</c> → <c>text.IndexOf(string)</c>
    /// </remarks>
    static void AnalyzeIndexOf_String_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument startIndexArgument)
    {
        if (startIndexArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.STRING_FQN.HasMethod(nameof(string.IndexOf), ParameterTypes.String, invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", startIndexArgument));
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
    void AnalyzeIndexOf_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (valueArgument.Value.TryGetStringConstant())
        {
            case "" when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always 0.", invocationExpression, "0"));
                break;

            case [var character] when PredefinedType.STRING_FQN.HasMethod(
                nameof(string.IndexOf),
                ParameterTypes.Char_StringComparison,
                valueArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        parameterNames is [var valueParameterName, _] ? valueParameterName : null,
                        character));
                break;

            default:
                if (invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
                {
                    switch (invocationExpression.Parent)
                    {
                        case IEqualityExpression equalityExpression when equalityExpression.LeftOperand == invocationExpression
                            && valueArgument.Value is { }
                            && comparisonTypeArgument.Value is { }:

                            switch (equalityExpression.EqualityType, equalityExpression.RightOperand.TryGetInt32Constant())
                            {
                                case (EqualityExpressionType.EQEQ, 0) when PredefinedType.STRING_FQN.HasMethod(
                                    nameof(string.StartsWith),
                                    ParameterTypes.String_StringComparison,
                                    invocationExpression.PsiModule):

                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.StartsWith)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.StartsWith),
                                            false,
                                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                            equalityExpression));
                                    break;

                                case (EqualityExpressionType.NE, 0) when PredefinedType.STRING_FQN.HasMethod(
                                    nameof(string.StartsWith),
                                    ParameterTypes.String_StringComparison,
                                    invocationExpression.PsiModule):

                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.StartsWith)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.StartsWith),
                                            true,
                                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                            equalityExpression));
                                    break;

                                case (EqualityExpressionType.NE, -1) when PredefinedType.STRING_FQN.HasMethod(
                                    nameof(string.Contains),
                                    ParameterTypes.String_StringComparison,
                                    invocationExpression.PsiModule):

                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.Contains)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.Contains),
                                            false,
                                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                            equalityExpression));
                                    break;

                                case (EqualityExpressionType.EQEQ, -1) when PredefinedType.STRING_FQN.HasMethod(
                                    nameof(string.Contains),
                                    ParameterTypes.String_StringComparison,
                                    invocationExpression.PsiModule):

                                    consumer.AddHighlighting(
                                        new UseOtherMethodSuggestion(
                                            $"Use the '{nameof(string.Contains)}' method.",
                                            invocationExpression,
                                            invokedExpression,
                                            nameof(string.Contains),
                                            true,
                                            [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                            equalityExpression));
                                    break;
                            }
                            break;

                        case IRelationalExpression relationalExpression when relationalExpression.LeftOperand == invocationExpression
                            && relationalExpression.RightOperand.TryGetInt32Constant() is { } value
                            && valueArgument.Value is { }
                            && comparisonTypeArgument.Value is { }
                            && PredefinedType.STRING_FQN.HasMethod(
                                nameof(string.Contains),
                                ParameterTypes.String_StringComparison,
                                invocationExpression.PsiModule):

                            var tokenType = relationalExpression.OperatorSign.GetTokenType();

                            if (tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                            {
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(string.Contains)}' method.",
                                        invocationExpression,
                                        invokedExpression,
                                        nameof(string.Contains),
                                        false,
                                        [valueArgument.Value.GetText(), comparisonTypeArgument.Value.GetText()],
                                        relationalExpression));
                            }

                            if (tokenType == CSharpTokenType.LT && value == 0)
                            {
                                consumer.AddHighlighting(
                                    new UseOtherMethodSuggestion(
                                        $"Use the '{nameof(string.Contains)}' method.",
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
                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(string, 0, StringComparison)</c> → <c>text.IndexOf(string, StringComparison)</c>
    /// </remarks>
    static void AnalyzeIndexOf_String_Int32_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument startIndexArgument)
    {
        if (startIndexArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.STRING_FQN.HasMethod(nameof(string.IndexOf), ParameterTypes.String_StringComparison, invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", startIndexArgument));
        }
    }

    /// <remarks>
    /// <c>text.IndexOfAny([])</c> → <c>-1</c><para/>
    /// <c>text.IndexOfAny([c])</c> → <c>text.IndexOf(c)</c><para/>
    /// <c>text.IndexOfAny(char[])</c> → <c>text.IndexOfAny(char[])</c>
    /// </remarks>
    void AnalyzeIndexOfAny_CharArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument anyOfArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (CollectionCreation.TryFrom(anyOfArgument.Value))
        {
            case { Count: 0 } when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always -1.", invocationExpression, "-1"));
                break;

            case { Count: 1 } collectionCreation
                when PredefinedType.STRING_FQN.HasMethod(nameof(string.IndexOf), ParameterTypes.Char, invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherMethodSuggestion(
                        $"Use the '{nameof(string.IndexOf)}' method.",
                        invocationExpression,
                        invokedExpression,
                        nameof(string.IndexOf),
                        false,
                        [collectionCreation.SingleElement.GetText()]));
                break;

            case { Count: > 1 } collectionCreation:
                var set = new HashSet<char>(collectionCreation.Count);

                foreach (var (element, character) in collectionCreation.ElementsWithCharConstants)
                {
                    if (!set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                    }
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOfAny(char[], 0)</c> → <c>text.IndexOfAny(char[])</c><para/>
    /// <c>text.IndexOfAny([c], int)</c> → <c>text.IndexOf(c, int)</c><para/>
    /// <c>text.IndexOfAny(char[], int)</c> → <c>text.IndexOfAny(char[], int)</c>
    /// </remarks>
    static void AnalyzeIndexOfAny_CharArray_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument anyOfArgument,
        ICSharpArgument startIndexArgument)
    {
        if (startIndexArgument.Value.TryGetInt32Constant() == 0
            && PredefinedType.STRING_FQN.HasMethod(nameof(string.IndexOfAny), ParameterTypes.CharArray, invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(new RedundantArgumentHint("Passing 0 is redundant.", startIndexArgument));
            return;
        }

        switch (CollectionCreation.TryFrom(anyOfArgument.Value))
        {
            case { Count: 1 } collectionCreation when startIndexArgument.Value is { }
                && PredefinedType.STRING_FQN.HasMethod(nameof(string.IndexOf), ParameterTypes.Char_Int32, invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherMethodSuggestion(
                        $"Use the '{nameof(string.IndexOf)}' method.",
                        invocationExpression,
                        invokedExpression,
                        nameof(string.IndexOf),
                        false,
                        [collectionCreation.SingleElement.GetText(), startIndexArgument.Value.GetText()]));
                break;

            case { Count: > 1 } collectionCreation:
                var set = new HashSet<char>(collectionCreation.Count);

                foreach (var (element, character) in collectionCreation.ElementsWithCharConstants)
                {
                    if (!set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                    }
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOfAny([c], int, int)</c> → <c>text.IndexOf(c, int)</c><para/>
    /// <c>text.IndexOfAny(char[], int, int)</c> → <c>text.IndexOfAny(char[], int, int)</c>
    /// </remarks>
    static void AnalyzeIndexOfAny_CharArray_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument anyOfArgument,
        ICSharpArgument startIndexArgument,
        ICSharpArgument countArgument)
    {
        switch (CollectionCreation.TryFrom(anyOfArgument.Value))
        {
            case { Count: 1 } collectionCreation when startIndexArgument.Value is { }
                && countArgument.Value is { }
                && PredefinedType.STRING_FQN.HasMethod(nameof(string.IndexOf), ParameterTypes.Char_Int32_Int32, invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherMethodSuggestion(
                        $"Use the '{nameof(string.IndexOf)}' method.",
                        invocationExpression,
                        invokedExpression,
                        nameof(string.IndexOf),
                        false,
                        [collectionCreation.SingleElement.GetText(), startIndexArgument.Value.GetText(), countArgument.Value.GetText()]));
                break;

            case { Count: > 1 } collectionCreation:
                var set = new HashSet<char>(collectionCreation.Count);

                foreach (var (element, character) in collectionCreation.ElementsWithCharConstants)
                {
                    if (!set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                    }
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.LastIndexOf(char, 0)</c> → <c>-1</c>
    /// </remarks>
    void AnalyzeLastIndexOf_Char_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument startIndexArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
            && startIndexArgument.Value.TryGetInt32Constant() == 0)
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always -1.", invocationExpression, "-1"));
        }
    }

    /// <remarks>
    /// <c>text.LastIndexOf("")</c> → <c>text.Length</c> (.NET 5)
    /// </remarks>
    static void AnalyzeLastIndexOf_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (invocationExpression.PsiModule.TargetFrameworkId.Version.Major >= 5
            && !invocationExpression.IsUsedAsStatement()
            && valueArgument.Value.TryGetStringConstant() == "")
        {
            consumer.AddHighlighting(
                new UseStringPropertySuggestion(
                    $"Use the '{nameof(string.Length)}' property.",
                    invocationExpression,
                    invokedExpression,
                    nameof(string.Length)));
        }
    }

    /// <remarks>
    /// <c>text.LastIndexOf("", StringComparison)</c> → <c>text.Length</c><para/> (.NET 5)
    /// <c>text.LastIndexOf(string, Ordinal)</c> → <c>text.LastIndexOf(char)</c>
    /// </remarks>
    static void AnalyzeLastIndexOf_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        switch (valueArgument.Value.TryGetStringConstant())
        {
            case "" when invocationExpression.PsiModule.TargetFrameworkId.Version.Major >= 5 && !invocationExpression.IsUsedAsStatement():
                consumer.AddHighlighting(
                    new UseStringPropertySuggestion(
                        $"Use the '{nameof(string.Length)}' property.",
                        invocationExpression,
                        invokedExpression,
                        nameof(string.Length)));
                break;

            case [var character] when comparisonTypeArgument.Value.TryGetStringComparisonConstant() == StringComparison.Ordinal
                && PredefinedType.STRING_FQN.HasMethod(
                    nameof(string.LastIndexOf),
                    ParameterTypes.Char,
                    valueArgument.NameIdentifier is { },
                    out var parameterNames,
                    invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character.",
                        valueArgument,
                        parameterNames is [var valueParameterName] ? valueParameterName : null,
                        character,
                        redundantArguments: [comparisonTypeArgument]));
                break;
        }
    }

    /// <remarks>
    /// <c>text.PadLeft(0)</c> → <c>text</c>
    /// </remarks>
    static void AnalyzePadLeft_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument totalWidthArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && totalWidthArgument.Value.TryGetInt32Constant() == 0)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(string.PadLeft)}' with 0 is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>text.PadLeft(0, char)</c> → <c>text</c><para/>
    /// <c>text.PadLeft(int, ' ')</c> → <c>text.PadLeft(int)</c>
    /// </remarks>
    static void AnalyzePadLeft_Int32_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument totalWidthArgument,
        ICSharpArgument paddingCharArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            if (totalWidthArgument.Value.TryGetInt32Constant() == 0)
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(string.PadLeft)}' with 0 is redundant.",
                        invocationExpression,
                        invokedExpression));
                return;
            }

            if (paddingCharArgument.Value.TryGetCharConstant() == ' '
                && PredefinedType.STRING_FQN.HasMethod(nameof(string.PadLeft), ParameterTypes.Int32, invokedExpression.GetPsiModule()))
            {
                consumer.AddHighlighting(new RedundantArgumentHint("Passing ' ' is redundant.", paddingCharArgument));
            }
        }
    }

    /// <remarks>
    /// <c>text.PadRight(0)</c> → <c>text</c>
    /// </remarks>
    static void AnalyzePadRight_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument totalWidthArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && totalWidthArgument.Value.TryGetInt32Constant() == 0)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(string.PadRight)}' with 0 is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>text.PadRight(0, char)</c> → <c>text</c><para/>
    /// <c>text.PadRight(int, ' ')</c> → <c>text.PadRight(int)</c>
    /// </remarks>
    static void AnalyzePadRight_Int32_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument totalWidthArgument,
        ICSharpArgument paddingCharArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            if (totalWidthArgument.Value.TryGetInt32Constant() == 0)
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(string.PadRight)}' with 0 is redundant.",
                        invocationExpression,
                        invokedExpression));
                return;
            }

            if (paddingCharArgument.Value.TryGetCharConstant() == ' '
                && PredefinedType.STRING_FQN.HasMethod(nameof(string.PadRight), ParameterTypes.Int32, invokedExpression.GetPsiModule()))
            {
                consumer.AddHighlighting(new RedundantArgumentHint("Passing ' ' is redundant.", paddingCharArgument));
            }
        }
    }

    /// <remarks>
    /// <c>text.Remove(0)</c> → <c>""</c> (.NET 6)<para/>
    /// <c>text.Remove(int)</c> → <c>text[..startIndex]</c> (C# 8)
    /// </remarks>
    void AnalyzeRemove_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument startIndexArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement())
        {
            if (startIndexArgument.Value.TryGetInt32Constant() == 0)
            {
                if (invocationExpression.PsiModule.TargetFrameworkId.Version.Major >= 6
                    && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
                {
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                }

                return;
            }

            if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp80 && startIndexArgument.Value is { })
            {
                consumer.AddHighlighting(
                    new UseRangeIndexerSuggestion(
                        "Use the range indexer.",
                        invocationExpression,
                        invokedExpression,
                        "",
                        startIndexArgument.Value.GetText()));
            }
        }
    }

    /// <remarks>
    /// <c>text.Remove(0, int)</c> → <c>text[count..]</c> (C# 8)
    /// </remarks>
    static void AnalyzeRemove_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument startIndexArgument,
        ICSharpArgument countArgument)
    {
        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp80
            && !invocationExpression.IsUsedAsStatement()
            && startIndexArgument.Value.TryGetInt32Constant() == 0
            && countArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseRangeIndexerSuggestion(
                    "Use the range indexer.",
                    invocationExpression,
                    invokedExpression,
                    countArgument.Value.GetText(),
                    ""));
        }
    }

    /// <remarks>
    /// <c>text.Replace(string, string, Ordinal)</c> → <c>text</c><para/>
    /// <c>text.Replace(string, string, Ordinal)</c> → <c>text.Replace(char, char)</c>
    /// </remarks>
    static void AnalyzeReplace_String_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument oldValueArgument,
        ICSharpArgument newValueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        if (oldValueArgument.Value.TryGetStringConstant() is { } oldValue and not ""
            && newValueArgument.Value.TryGetStringConstant() is { } newValue
            && comparisonTypeArgument.Value.TryGetStringComparisonConstant() == StringComparison.Ordinal)
        {
            if (!invocationExpression.IsUsedAsStatement() && oldValue == newValue)
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(string.Replace)}' with identical values is redundant.",
                        invocationExpression,
                        invokedExpression));
                return;
            }

            if (oldValue is [var oldCharacter]
                && newValue is [var newCharacter]
                && PredefinedType.STRING_FQN.HasMethod(
                    nameof(string.Replace),
                    ParameterTypes.Char_Char,
                    oldValueArgument.NameIdentifier is { } || newValueArgument.NameIdentifier is { },
                    out var parameterNames,
                    invocationExpression.PsiModule))
            {
                var highlighting = new PassSingleCharactersSuggestion(
                    "Pass the single character.",
                    [oldValueArgument, newValueArgument],
                    parameterNames is [var oldCharParameterName, var newCharParameterName]
                        ?
                        [
                            oldValueArgument.NameIdentifier is { } ? oldCharParameterName : null,
                            newValueArgument.NameIdentifier is { } ? newCharParameterName : null,
                        ]
                        : new string?[2],
                    [oldCharacter, newCharacter],
                    comparisonTypeArgument);

                consumer.AddHighlighting(highlighting, oldValueArgument.Value.GetDocumentRange());
                consumer.AddHighlighting(highlighting, newValueArgument.Value.GetDocumentRange());
            }
        }
    }

    /// <remarks>
    /// <c>text.Replace(char, char)</c> → <c>text</c>
    /// </remarks>
    static void AnalyzeReplace_Char_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument oldCharArgument,
        ICSharpArgument newCharArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && oldCharArgument.Value.TryGetCharConstant() is { } oldCharacter
            && newCharArgument.Value.TryGetCharConstant() is { } newCharacter
            && oldCharacter == newCharacter)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(string.Replace)}' with identical characters is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>text.Replace(string, string)</c> → <c>text</c><para/>
    /// <c>text.Replace(string, string)</c> → <c>text.Replace(char, char)</c>
    /// </remarks>
    static void AnalyzeReplace_String_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument oldValueArgument,
        ICSharpArgument newValueArgument)
    {
        if (oldValueArgument.Value.TryGetStringConstant() is { } oldValue and not "" && newValueArgument.Value.TryGetStringConstant() is { } newValue)
        {
            if (!invocationExpression.IsUsedAsStatement() && oldValue == newValue)
            {
                consumer.AddHighlighting(
                    new RedundantMethodInvocationHint(
                        $"Calling '{nameof(string.Replace)}' with identical values is redundant.",
                        invocationExpression,
                        invokedExpression));
                return;
            }

            if (oldValue is [var oldCharacter]
                && newValue is [var newCharacter]
                && PredefinedType.STRING_FQN.HasMethod(
                    nameof(string.Replace),
                    ParameterTypes.Char_Char,
                    oldValueArgument.NameIdentifier is { } || newValueArgument.NameIdentifier is { },
                    out var parameterNames,
                    invocationExpression.PsiModule))
            {
                var highlighting = new PassSingleCharactersSuggestion(
                    "Pass the single character.",
                    [oldValueArgument, newValueArgument],
                    parameterNames is [var oldCharParameterName, var newCharParameterName]
                        ?
                        [
                            oldValueArgument.NameIdentifier is { } ? oldCharParameterName : null,
                            newValueArgument.NameIdentifier is { } ? newCharParameterName : null,
                        ]
                        : new string?[2],
                    [oldCharacter, newCharacter]);

                consumer.AddHighlighting(highlighting, oldValueArgument.Value.GetDocumentRange());
                consumer.AddHighlighting(highlighting, newValueArgument.Value.GetDocumentRange());
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(char, 0, StringSplitOptions)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(char, 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(char, 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)
    /// </remarks>
    void AnalyzeSplit_Char_Int32_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument countArgument,
        ICSharpArgument? optionsArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            switch (countArgument.Value.TryGetInt32Constant())
            {
                case 0:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always an empty array.",
                            invocationExpression,
                            CreateStringArray([], invocationExpression)));
                    break;

                case 1:
                    Debug.Assert(invokedExpression.QualifierExpression is { });

                    switch (optionsArgument is { } ? optionsArgument.Value.TryGetStringSplitOptionsConstant() : StringSplitOptions.None)
                    {
                        case StringSplitOptions.None:
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always an array with a single element.",
                                    invocationExpression,
                                    CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                            break;

                        case (StringSplitOptions)2 when PredefinedType.STRING_FQN.HasMethod(nameof(string.Trim), [], invocationExpression.PsiModule): // todo: use StringSplitOptions.TrimEntries
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always an array with a single trimmed element.",
                                    invocationExpression,
                                    CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                            break;
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(char[])</c> → <c>text.Split(char[])</c>
    /// </remarks>
    static void AnalyzeSplit_CharArray(IHighlightingConsumer consumer, TreeNodeCollection<ICSharpArgument> arguments)
    {
        switch (arguments)
        {
            case [_, _, ..]:
            {
                var set = new HashSet<char>(arguments.Count);

                foreach (var argument in arguments)
                {
                    if (argument.Value.TryGetCharConstant() is { } character && !set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantArgumentHint("The character is already passed.", argument));
                    }
                }

                break;
            }

            case [{ Value: var argumentExpression }] when CollectionCreation.TryFrom(argumentExpression) is { Count: > 0 } collectionCreation:
            {
                var set = new HashSet<char>(collectionCreation.Count);

                foreach (var (element, character) in collectionCreation.ElementsWithCharConstants)
                {
                    if (!set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                    }
                }

                break;
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(char[], 0)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(char[], 1)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(char[], int)</c> → <c>text.Split(char[], int)</c><para/>
    /// </remarks>
    void AnalyzeSplit_CharArray_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument countArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (separatorArgument.Value, countArgument.Value.TryGetInt32Constant())
        {
            case (_, 0) when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateStringArray([], invocationExpression)));
                break;

            case (_, 1) when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                Debug.Assert(invokedExpression.QualifierExpression is { });

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an array with a single element.",
                        invocationExpression,
                        CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                break;

            case (_, _) when CollectionCreation.TryFrom(separatorArgument.Value) is { Count: > 0 } collectionCreation:
            {
                var set = new HashSet<char>(collectionCreation.Count);

                foreach (var (element, character) in collectionCreation.ElementsWithCharConstants)
                {
                    if (!set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                    }
                }

                break;
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(char[], StringSplitOptions)</c> → <c>text.Split(char[], StringSplitOptions)</c><para/>
    /// </remarks>
    static void AnalyzeSplit_CharArray_StringSplitOptions(IHighlightingConsumer consumer, ICSharpArgument separatorArgument)
    {
        if (CollectionCreation.TryFrom(separatorArgument.Value) is { Count: > 0 } collectionCreation)
        {
            var set = new HashSet<char>(collectionCreation.Count);

            foreach (var (element, character) in collectionCreation.ElementsWithCharConstants)
            {
                if (!set.Add(character))
                {
                    consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                }
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(char[], 0, StringSplitOptions)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(char[], 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(char[], 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(char[], int, StringSplitOptions)</c> → <c>text.Split(char[], int, StringSplitOptions)</c><para/>
    /// </remarks>
    void AnalyzeSplit_CharArray_Int32_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument countArgument,
        ICSharpArgument optionsArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (separatorArgument.Value, countArgument.Value.TryGetInt32Constant())
        {
            case (_, 0) when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateStringArray([], invocationExpression)));
                break;

            case (_, 1) when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                Debug.Assert(invokedExpression.QualifierExpression is { });

                switch (optionsArgument.Value.TryGetStringSplitOptionsConstant())
                {
                    case StringSplitOptions.None:
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single element.",
                                invocationExpression,
                                CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                        break;

                    case (StringSplitOptions)2 when PredefinedType.STRING_FQN.HasMethod(nameof(string.Trim), [], invocationExpression.PsiModule): // todo: use StringSplitOptions.TrimEntries
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single trimmed element.",
                                invocationExpression,
                                CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                        break;
                }
                break;

            case (_, _) when CollectionCreation.TryFrom(separatorArgument.Value) is { Count: > 0 } collectionCreation:
            {
                var set = new HashSet<char>(collectionCreation.Count);

                foreach (var (element, character) in collectionCreation.ElementsWithCharConstants)
                {
                    if (!set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                    }
                }

                break;
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(null, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split("", None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(null, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split("", TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(string, StringSplitOptions)</c> → <c>text.Split(char, StringSplitOptions)</c>
    /// </remarks>
    void AnalyzeSplit_String_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument? optionsArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        var separator = separatorArgument.Value.IsDefaultValue() ? "" : separatorArgument.Value.TryGetStringConstant();

        switch (separator)
        {
            case "" when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                Debug.Assert(invokedExpression.QualifierExpression is { });

                switch (optionsArgument is { } ? optionsArgument.Value.TryGetStringSplitOptionsConstant() : StringSplitOptions.None)
                {
                    case StringSplitOptions.None:
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single element.",
                                invocationExpression,
                                CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                        break;

                    case (StringSplitOptions)2 when PredefinedType.STRING_FQN.HasMethod(nameof(string.Trim), [], invocationExpression.PsiModule): // todo: use StringSplitOptions.TrimEntries
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single trimmed element.",
                                invocationExpression,
                                CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                        break;
                }
                break;

            case [var character] when PredefinedType.STRING_FQN.HasMethod(
                nameof(string.Split),
                ParameterTypes.Char_StringSplitOptions,
                separatorArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character",
                        separatorArgument,
                        parameterNames is [var separatorParameterName, _] ? separatorParameterName : null,
                        character));
                break;
        }
    }

    /// <remarks>
    /// <c>text.Split(string, 0, StringSplitOptions)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(string, 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(string, 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(null, int, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split("", int, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(null, int, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split("", int, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(string, int, StringSplitOptions)</c> → <c>text.Split(char, int, StringSplitOptions)</c>
    /// </remarks>
    void AnalyzeSplit_String_Int32_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument countArgument,
        ICSharpArgument? optionsArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        var separator = separatorArgument.Value.IsDefaultValue() ? "" : separatorArgument.Value.TryGetStringConstant();

        switch (separator, countArgument.Value.TryGetInt32Constant())
        {
            case (_, 0) when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateStringArray([], invocationExpression)));
                break;

            case (_, 1) or ("", _) when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                Debug.Assert(invokedExpression.QualifierExpression is { });

                switch (optionsArgument is { } ? optionsArgument.Value.TryGetStringSplitOptionsConstant() : StringSplitOptions.None)
                {
                    case StringSplitOptions.None:
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single element.",
                                invocationExpression,
                                CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                        break;

                    case (StringSplitOptions)2 when PredefinedType.STRING_FQN.HasMethod(nameof(string.Trim), [], invocationExpression.PsiModule): // todo: use StringSplitOptions.TrimEntries
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single trimmed element.",
                                invocationExpression,
                                CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                        break;
                }
                break;

            case ([var character], _) when PredefinedType.STRING_FQN.HasMethod(
                nameof(string.Split),
                ParameterTypes.Char_Int32_StringSplitOptions,
                separatorArgument.NameIdentifier is { },
                out var parameterNames,
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new PassSingleCharacterSuggestion(
                        "Pass the single character",
                        separatorArgument,
                        parameterNames is [var separatorParameterName, _, _] ? separatorParameterName : null,
                        character));
                break;
        }
    }

    /// <remarks>
    /// <c>text.Split([""], None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split([""], TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(string[], StringSplitOptions)</c> → <c>text.Split(char[], StringSplitOptions)</c><para/>
    /// <c>text.Split(string[], StringSplitOptions)</c> → <c>text.Split(string[], StringSplitOptions)</c>
    /// </remarks>
    void AnalyzeSplit_StringArray_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument optionsArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (CollectionCreation.TryFrom(separatorArgument.Value) is { Count: > 0 } collectionCreation)
        {
            if (collectionCreation.StringConstants is [""])
            {
                if (!invocationExpression.IsUsedAsStatement()
                    && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
                {
                    Debug.Assert(invokedExpression.QualifierExpression is { });

                    switch (optionsArgument.Value.TryGetStringSplitOptionsConstant())
                    {
                        case StringSplitOptions.None:
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always an array with a single element.",
                                    invocationExpression,
                                    CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                            break;

                        case (StringSplitOptions)2 when PredefinedType.STRING_FQN.HasMethod(nameof(string.Trim), [], invocationExpression.PsiModule): // todo: use StringSplitOptions.TrimEntries
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always an array with a single trimmed element.",
                                    invocationExpression,
                                    CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                            break;
                    }
                }
            }
            else
            {
                if (collectionCreation.StringConstants.All(s => s is [_]))
                {
                    if (PredefinedType.STRING_FQN.HasMethod(
                        nameof(string.Split),
                        ParameterTypes.CharArray_StringSplitOptions,
                        invocationExpression.PsiModule))
                    {
                        var highlighting = collectionCreation.Expression switch
                        {
                            ICollectionExpression collectionExpression => new PassSingleCharactersSuggestion(
                                "Pass the single character",
                                [..collectionExpression.CollectionElements],
                                [..from s in collectionCreation.StringConstants select s[0]]),

                            IArrayCreationExpression arrayCreationExpression => new PassSingleCharactersSuggestion(
                                "Pass the single character",
                                arrayCreationExpression,
                                [..from s in collectionCreation.StringConstants select s[0]]),

                            _ => throw new NotSupportedException(),
                        };

                        foreach (var element in collectionCreation.Elements)
                        {
                            consumer.AddHighlighting(highlighting, element.GetDocumentRange());
                        }
                    }
                }
                else
                {
                    var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

                    foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
                    {
                        if (!set.Add(s))
                        {
                            consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                        }
                    }
                }
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(string[], 0, StringSplitOptions)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(string[], 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(string[], 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split([""], int, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split([""], int, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(string[], int, StringSplitOptions)</c> → <c>text.Split(char[], int, StringSplitOptions)</c><para/>
    /// <c>text.Split(string[], int, StringSplitOptions)</c> → <c>text.Split(string[], int, StringSplitOptions)</c>
    /// </remarks>
    void AnalyzeSplit_StringArray_Int32_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument countArgument,
        ICSharpArgument optionsArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (CollectionCreation.TryFrom(separatorArgument.Value), countArgument.Value.TryGetInt32Constant())
        {
            case (_, 0) when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateStringArray([], invocationExpression)));
                break;

            case (_, 1) or ({ StringConstants: [""] }, _) when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                Debug.Assert(invokedExpression.QualifierExpression is { });

                switch (optionsArgument.Value.TryGetStringSplitOptionsConstant())
                {
                    case StringSplitOptions.None:
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single element.",
                                invocationExpression,
                                CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                        break;

                    case (StringSplitOptions)2: // todo: use StringSplitOptions.TrimEntries
                        consumer.AddHighlighting(
                            new UseExpressionResultSuggestion(
                                "The expression is always an array with a single trimmed element.",
                                invocationExpression,
                                CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                        break;
                }
                break;

            case ({ Count: > 0 } collectionCreation, _):
                if (collectionCreation.StringConstants.All(s => s is [_]))
                {
                    if (PredefinedType.STRING_FQN.HasMethod(
                        nameof(string.Split),
                        ParameterTypes.CharArray_Int32_StringSplitOptions,
                        invocationExpression.PsiModule))
                    {
                        var highlighting = collectionCreation.Expression switch
                        {
                            ICollectionExpression collectionExpression => new PassSingleCharactersSuggestion(
                                "Pass the single character",
                                [..collectionExpression.CollectionElements],
                                [..from s in collectionCreation.StringConstants select s[0]]),

                            IArrayCreationExpression arrayCreationExpression => new PassSingleCharactersSuggestion(
                                "Pass the single character",
                                arrayCreationExpression,
                                [..from s in collectionCreation.StringConstants select s[0]]),

                            _ => throw new NotSupportedException(),
                        };

                        foreach (var element in collectionCreation.Elements)
                        {
                            consumer.AddHighlighting(highlighting, element.GetDocumentRange());
                        }
                    }
                }
                else
                {
                    var set = new HashSet<string>(collectionCreation.Count, StringComparer.Ordinal);

                    foreach (var (element, s) in collectionCreation.ElementsWithStringConstants)
                    {
                        if (!set.Add(s))
                        {
                            consumer.AddHighlighting(new RedundantElementHint("The string is already passed.", element));
                        }
                    }
                }
                break;
        }
    }

    /// <remarks>
    /// <c>text.StartsWith(char)</c> → <c>text is [var firstChar, ..] &amp;&amp; firstChar == value</c> (C# 11)<para/>
    /// <c>text.StartsWith(char)</c> → <c>text is [value, ..]</c> (C# 11)
    /// </remarks>
    void AnalyzeStartsWith_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
            && valueArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseStringListPatternSuggestion(
                    "Use list pattern.",
                    invocationExpression,
                    invokedExpression,
                    ListPatternSuggestionKind.FirstCharacter,
                    [(valueArgument.Value.GetText(), valueArgument.Value.TryGetCharConstant() is { })]));
        }
    }

    /// <remarks>
    /// <c>text.StartsWith("")</c> → <c>true</c>
    /// </remarks>
    void AnalyzeStartsWith_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
            && valueArgument.Value.TryGetStringConstant() == "")
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
        }
    }

    /// <remarks>
    /// <c>text.StartsWith("", StringComparison)</c> → <c>true</c><para/>
    /// <c>text.StartsWith(string, Ordinal)</c> → <c>text is [c, ..]</c> (C# 11)<para/>
    /// <c>text.StartsWith(string, OrdinalIgnoresCase)</c> → <c>text is [l or u, ..]</c> (C# 11)
    /// </remarks>
    void AnalyzeStartsWith_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            switch (valueArgument.Value.TryGetStringConstant())
            {
                case "":
                    consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
                    break;

                case [var character] when invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110:
                    switch (comparisonTypeArgument.Value.TryGetStringComparisonConstant())
                    {
                        case StringComparison.Ordinal:
                            consumer.AddHighlighting(
                                new UseStringListPatternSuggestion(
                                    "Use list pattern.",
                                    invocationExpression,
                                    invokedExpression,
                                    ListPatternSuggestionKind.FirstCharacter,
                                    [($"'{character}'", true)]));
                            break;

                        case StringComparison.OrdinalIgnoreCase:
                            var lowerCaseCharacter = char.ToLowerInvariant(character);
                            var upperCaseCharacter = char.ToUpperInvariant(character);

                            if (lowerCaseCharacter == upperCaseCharacter)
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern.",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.FirstCharacter,
                                        [($"'{lowerCaseCharacter}'", true)]));
                            }
                            else
                            {
                                consumer.AddHighlighting(
                                    new UseStringListPatternSuggestion(
                                        "Use list pattern.",
                                        invocationExpression,
                                        invokedExpression,
                                        ListPatternSuggestionKind.FirstCharacter,
                                        [($"'{lowerCaseCharacter}'", true), ($"'{upperCaseCharacter}'", true)]));
                            }
                            break;
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>text.Substring(0)</c> → <c>text</c>
    /// </remarks>
    static void AnalyzeSubstring_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument startIndexArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && startIndexArgument.Value.TryGetInt32Constant() == 0)
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(string.Substring)}' with 0 is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>text.ToString(IFormatProvider)</c> → <c>text</c>
    /// </remarks>
    static void AnalyzeToString_IFormatProvider(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            consumer.AddHighlighting(
                new RedundantMethodInvocationHint(
                    $"Calling '{nameof(string.ToString)}' with a format provider is redundant.",
                    invocationExpression,
                    invokedExpression));
        }
    }

    /// <remarks>
    /// <c>text.Trim(null)</c> → <c>text.Trim()</c><para/>
    /// <c>text.Trim([])</c> → <c>text.Trim()</c><para/>
    /// <c>text.Trim(new char[0])</c> → <c>text.Trim()</c><para/>
    /// <c>text.Trim(new char[] { })</c> → <c>text.Trim()</c><para/>
    /// <c>text.Trim(Array.Empty&lt;char&gt;())</c> → <c>text.Trim()</c><para/>
    /// <c>text.Trim(c, c)</c> → <c>text.Trim(c)</c><para/>
    /// <c>text.Trim(char[])</c> → <c>text.Trim(char[])</c><para/>
    /// </remarks>
    static void AnalyzeTrim_CharArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument> arguments)
    {
        switch (arguments)
        {
            case [_, _, ..]:
            {
                var set = new HashSet<char>(arguments.Count);

                foreach (var argument in arguments)
                {
                    if (argument.Value.TryGetCharConstant() is { } character && !set.Add(character))
                    {
                        consumer.AddHighlighting(new RedundantArgumentHint("The character is already passed.", argument));
                    }
                }

                break;
            }

            case [{ } argument] when CollectionCreation.TryFrom(argument.Value) is { } collectionCreation:
            {
                if (collectionCreation.Count > 0)
                {
                    var set = new HashSet<char>(collectionCreation.Count);

                    foreach (var (element, character) in collectionCreation.ElementsWithCharConstants)
                    {
                        if (!set.Add(character))
                        {
                            consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                        }
                    }
                }
                else
                {
                    if (PredefinedType.STRING_FQN.HasMethod(nameof(string.Trim), [], invocationExpression.PsiModule))
                    {
                        consumer.AddHighlighting(new RedundantArgumentHint("Passing an empty array is redundant.", argument));
                    }
                }

                break;
            }

            case [{ } argument] when argument.Value.IsDefaultValue()
                && PredefinedType.STRING_FQN.HasMethod(nameof(string.Trim), [], invocationExpression.PsiModule):

                consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", argument));
                break;
        }
    }

    /// <remarks>
    /// <c>text.TrimEnd(null)</c> → <c>text.TrimEnd()</c><para/>
    /// <c>text.TrimEnd([])</c> → <c>text.TrimEnd()</c><para/>
    /// <c>text.TrimEnd(new char[0])</c> → <c>text.TrimEnd()</c><para/>
    /// <c>text.TrimEnd(new char[] { })</c> → <c>text.TrimEnd()</c><para/>
    /// <c>text.TrimEnd(Array.Empty&lt;char&gt;())</c> → <c>text.TrimEnd()</c><para/>
    /// <c>text.TrimEnd(c, c)</c> → <c>text.TrimEnd(c)</c><para/>
    /// <c>text.TrimEnd(char[])</c> → <c>text.TrimEnd(char[])</c><para/>
    /// </remarks>
    static void AnalyzeTrimEnd_CharArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument> arguments)
    {
        switch (arguments)
        {
            case [_, _, ..]:
                {
                    var set = new HashSet<char>(arguments.Count);

                    foreach (var argument in arguments)
                    {
                        if (argument.Value.TryGetCharConstant() is { } character && !set.Add(character))
                        {
                            consumer.AddHighlighting(new RedundantArgumentHint("The character is already passed.", argument));
                        }
                    }

                    break;
                }

            case [{ } argument] when CollectionCreation.TryFrom(argument.Value) is { } collectionCreation:
                {
                    if (collectionCreation.Count > 0)
                    {
                        var set = new HashSet<char>(collectionCreation.Count);

                        foreach (var (element, character) in collectionCreation.ElementsWithCharConstants)
                        {
                            if (!set.Add(character))
                            {
                                consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                            }
                        }
                    }
                    else
                    {
                        if (PredefinedType.STRING_FQN.HasMethod(nameof(string.TrimEnd), [], invocationExpression.PsiModule))
                        {
                            consumer.AddHighlighting(new RedundantArgumentHint("Passing an empty array is redundant.", argument));
                        }
                    }

                    break;
                }

            case [{ } argument] when argument.Value.IsDefaultValue()
                && PredefinedType.STRING_FQN.HasMethod(nameof(string.TrimEnd), [], invocationExpression.PsiModule):

                consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", argument));
                break;
        }
    }

    /// <remarks>
    /// <c>text.TrimStart(null)</c> → <c>text.TrimStart()</c><para/>
    /// <c>text.TrimStart([])</c> → <c>text.TrimStart()</c><para/>
    /// <c>text.TrimStart(new char[0])</c> → <c>text.TrimStart()</c><para/>
    /// <c>text.TrimStart(new char[] { })</c> → <c>text.TrimStart()</c><para/>
    /// <c>text.TrimStart(Array.Empty&lt;char&gt;())</c> → <c>text.TrimStart()</c><para/>
    /// <c>text.TrimStart(c, c)</c> → <c>text.TrimStart(c)</c><para/>
    /// <c>text.TrimStart(char[])</c> → <c>text.TrimStart(char[])</c><para/>
    /// </remarks>
    static void AnalyzeTrimStart_CharArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument> arguments)
    {
        switch (arguments)
        {
            case [_, _, ..]:
                {
                    var set = new HashSet<char>(arguments.Count);

                    foreach (var argument in arguments)
                    {
                        if (argument.Value.TryGetCharConstant() is { } character && !set.Add(character))
                        {
                            consumer.AddHighlighting(new RedundantArgumentHint("The character is already passed.", argument));
                        }
                    }

                    break;
                }

            case [{ } argument] when CollectionCreation.TryFrom(argument.Value) is { } collectionCreation:
                {
                    if (collectionCreation.Count > 0)
                    {
                        var set = new HashSet<char>(collectionCreation.Count);

                        foreach (var (element, character) in collectionCreation.ElementsWithCharConstants)
                        {
                            if (!set.Add(character))
                            {
                                consumer.AddHighlighting(new RedundantElementHint("The character is already passed.", element));
                            }
                        }
                    }
                    else
                    {
                        if (PredefinedType.STRING_FQN.HasMethod(nameof(string.TrimStart), [], invocationExpression.PsiModule))
                        {
                            consumer.AddHighlighting(new RedundantArgumentHint("Passing an empty array is redundant.", argument));
                        }
                    }

                    break;
                }

            case [{ } argument] when argument.Value.IsDefaultValue()
                && PredefinedType.STRING_FQN.HasMethod(nameof(string.TrimStart), [], invocationExpression.PsiModule):

                consumer.AddHighlighting(new RedundantArgumentHint("Passing null is redundant.", argument));
                break;
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { QualifierExpression: { }, Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                IsStatic: false, TypeParameters: [], AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
            } method
            && method.ContainingType.IsSystemString())
        {
            switch (method.ShortName)
            {
                case nameof(string.Contains):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsString():
                            AnalyzeContains_String(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var stringComparisonType }], [var valueArgument, _])
                            when valueType.IsString() && IsStringComparison(stringComparisonType):

                            AnalyzeContains_String_StringComparison(consumer, element, invokedExpression, valueArgument);
                            break;
                    }
                    break;

                case nameof(string.EndsWith):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsChar():
                            AnalyzeEndsWith_Char(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsString():
                            AnalyzeEndsWith_String(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var stringComparisonType }], [var valueArgument, var comparisonTypeArgument])
                            when valueType.IsString() && IsStringComparison(stringComparisonType):

                            AnalyzeEndsWith_String_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                            break;
                    }
                    break;

                case nameof(string.IndexOf):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsChar():
                            AnalyzeIndexOf_Char(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var startIndexType }], [_, var startIndexArgument])
                            when valueType.IsChar() && startIndexType.IsInt():

                            AnalyzeIndexOf_Char_Int32(consumer, element, startIndexArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var stringComparisonType }], [var valueArgument, var comparisonTypeArgument])
                            when valueType.IsChar() && IsStringComparison(stringComparisonType):

                            AnalyzeIndexOf_Char_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                            break;

                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsString():
                            AnalyzeIndexOf_String(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var startIndexType }], [_, var startIndexArgument])
                            when valueType.IsString() && startIndexType.IsInt():

                            AnalyzeIndexOf_String_Int32(consumer, element, startIndexArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var stringComparisonType }], [var valueArgument, var comparisonTypeArgument])
                            when valueType.IsString() && IsStringComparison(stringComparisonType):

                            AnalyzeIndexOf_String_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var startIndexType }, { Type: var stringComparisonType }], [
                            _, var startIndexArgument, _,
                        ]) when valueType.IsString() && startIndexType.IsInt() && IsStringComparison(stringComparisonType):
                            AnalyzeIndexOf_String_Int32_StringComparison(consumer, element, startIndexArgument);
                            break;
                    }
                    break;

                case nameof(string.IndexOfAny):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var anyOfType }], [var anyOfArgument]) when anyOfType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element):
                            AnalyzeIndexOfAny_CharArray(consumer, element, invokedExpression, anyOfArgument);
                            break;

                        case ([{ Type: var anyOfType }, { Type: var startIndexType }], [var anyOfArgument, var startIndexArgument])
                            when anyOfType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element) && startIndexType.IsInt():

                            AnalyzeIndexOfAny_CharArray_Int32(consumer, element, invokedExpression, anyOfArgument, startIndexArgument);
                            break;

                        case ([{ Type: var anyOfType }, { Type: var startIndexType }, { Type: var countType }], [
                            var anyOfArgument, var startIndexArgument, var valueArgument
                        ]) when anyOfType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element) && startIndexType.IsInt() && countType.IsInt():

                            AnalyzeIndexOfAny_CharArray_Int32_Int32(
                                consumer,
                                element,
                                invokedExpression,
                                anyOfArgument,
                                startIndexArgument,
                                valueArgument);
                            break;
                    }
                    break;

                case nameof(string.LastIndexOf):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }, { Type: var startIndexType }], [_, var startIndexArgument])
                            when valueType.IsChar() && startIndexType.IsInt():

                            AnalyzeLastIndexOf_Char_Int32(consumer, element, invokedExpression, startIndexArgument);
                            break;

                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsString():
                            AnalyzeLastIndexOf_String(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var stringComparisonType }], [var valueArgument, var comparisonTypeArgument])
                            when valueType.IsString() && IsStringComparison(stringComparisonType):

                            AnalyzeLastIndexOf_String_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                            break;
                    }
                    break;

                case nameof(string.PadLeft):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var totalWidthType }], [var totalWidthArgument]) when totalWidthType.IsInt():
                            AnalyzePadLeft_Int32(consumer, element, invokedExpression, totalWidthArgument);
                            break;

                        case ([{ Type: var totalWidthType }, { Type: var paddingCharType }], [var totalWidthArgument, var paddingCharArgument])
                            when totalWidthType.IsInt() && paddingCharType.IsChar():

                            AnalyzePadLeft_Int32_Char(consumer, element, invokedExpression, totalWidthArgument, paddingCharArgument);
                            break;
                    }
                    break;

                case nameof(string.PadRight):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var totalWidthType }], [var totalWidthArgument]) when totalWidthType.IsInt():
                            AnalyzePadRight_Int32(consumer, element, invokedExpression, totalWidthArgument);
                            break;

                        case ([{ Type: var totalWidthType }, { Type: var paddingCharType }], [var totalWidthArgument, var paddingCharArgument])
                            when totalWidthType.IsInt() && paddingCharType.IsChar():

                            AnalyzePadRight_Int32_Char(consumer, element, invokedExpression, totalWidthArgument, paddingCharArgument);
                            break;
                    }
                    break;

                case nameof(string.Remove):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var startIndexType }], [var startIndexArgument]) when startIndexType.IsInt():
                            AnalyzeRemove_Int32(consumer, element, invokedExpression, startIndexArgument);
                            break;

                        case ([{ Type: var startIndexType }, { Type: var countType }], [var startIndexArgument, var countArgument])
                            when startIndexType.IsInt() && countType.IsInt():

                            AnalyzeRemove_Int32_Int32(consumer, element, invokedExpression, startIndexArgument, countArgument);
                            break;
                    }
                    break;

                case nameof(string.Replace):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var oldValueType }, { Type: var newValueType }, { Type: var stringComparisonType }], [
                            var oldValueArgument, var newValueArgument, var comparisonTypeArgument,
                        ]) when oldValueType.IsString() && newValueType.IsString() && IsStringComparison(stringComparisonType):
                            AnalyzeReplace_String_String_StringComparison(
                                consumer,
                                element,
                                invokedExpression,
                                oldValueArgument,
                                newValueArgument,
                                comparisonTypeArgument);
                            break;

                        case ([{ Type: var oldCharType }, { Type: var newCharType }], [var oldCharArgument, var newCharArgument])
                            when oldCharType.IsChar() && newCharType.IsChar():

                            AnalyzeReplace_Char_Char(consumer, element, invokedExpression, oldCharArgument, newCharArgument);
                            break;

                        case ([{ Type: var oldValueType }, { Type: var newValueType }], [var oldValueArgument, var newValueArgument])
                            when oldValueType.IsString() && newValueType.IsString():

                            AnalyzeReplace_String_String(consumer, element, invokedExpression, oldValueArgument, newValueArgument);
                            break;
                    }
                    break;

                case nameof(string.Split):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], { Count: 2 or 3 } arguments)
                            when separatorType.IsChar() && countType.IsInt() && IsStringSplitOptions(optionsType):

                            AnalyzeSplit_Char_Int32_StringSplitOptions(
                                consumer,
                                element,
                                invokedExpression,
                                arguments[1],
                                arguments.Count == 3 ? arguments[^1] : null);
                            break;

                        case ([{ Type: var separatorType }], var arguments) when separatorType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element):
                            AnalyzeSplit_CharArray(consumer, arguments);
                            break;

                        case ([{ Type: var separatorType }, { Type: var countType }], [var separatorArgument, var countArgument])
                            when separatorType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element) && countType.IsInt():

                            AnalyzeSplit_CharArray_Int32(consumer, element, invokedExpression, separatorArgument, countArgument);
                            break;

                        case ([{ Type: var separatorType }, { Type: var optionsType }], [var separatorArgument, _])
                            when separatorType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element) && IsStringSplitOptions(optionsType):

                            AnalyzeSplit_CharArray_StringSplitOptions(consumer, separatorArgument);
                            break;

                        case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], [
                                var separatorArgument, var countArgument, var optionsArgument,
                            ]) when separatorType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element)
                            && countType.IsInt()
                            && IsStringSplitOptions(optionsType):

                            AnalyzeSplit_CharArray_Int32_StringSplitOptions(
                                consumer,
                                element,
                                invokedExpression,
                                separatorArgument,
                                countArgument,
                                optionsArgument);
                            break;

                        case ([{ Type: var separatorType }, { Type: var optionsType }], { Count: 1 or 2 } arguments)
                            when separatorType.IsString() && IsStringSplitOptions(optionsType):

                            AnalyzeSplit_String_StringSplitOptions(
                                consumer,
                                element,
                                invokedExpression,
                                arguments[0],
                                arguments.Count == 2 ? arguments[^1] : null);
                            break;

                        case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], { Count: 2 or 3 } arguments)
                            when separatorType.IsString() && countType.IsInt() && IsStringSplitOptions(optionsType):

                            AnalyzeSplit_String_Int32_StringSplitOptions(
                                consumer,
                                element,
                                invokedExpression,
                                arguments[0],
                                arguments[1],
                                arguments.Count == 3 ? arguments[^1] : null);
                            break;

                        case ([{ Type: var separatorType }, { Type: var optionsType }], [var separatorArgument, var optionsArgument])
                            when separatorType.IsGenericArrayOf(PredefinedType.STRING_FQN, element) && IsStringSplitOptions(optionsType):

                            AnalyzeSplit_StringArray_StringSplitOptions(consumer, element, invokedExpression, separatorArgument, optionsArgument);
                            break;

                        case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], [
                                var separatorArgument, var countArgument, var optionsArgument,
                            ]) when separatorType.IsGenericArrayOf(PredefinedType.STRING_FQN, element)
                            && countType.IsInt()
                            && IsStringSplitOptions(optionsType):

                            AnalyzeSplit_StringArray_Int32_StringSplitOptions(
                                consumer,
                                element,
                                invokedExpression,
                                separatorArgument,
                                countArgument,
                                optionsArgument);
                            break;
                    }
                    break;

                case nameof(string.StartsWith):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsChar():
                            AnalyzeStartsWith_Char(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsString():
                            AnalyzeStartsWith_String(consumer, element, invokedExpression, valueArgument);
                            break;

                        case ([{ Type: var valueType }, { Type: var stringComparisonType }], [var valueArgument, var comparisonTypeArgument])
                            when valueType.IsString() && IsStringComparison(stringComparisonType):

                            AnalyzeStartsWith_String_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                            break;
                    }
                    break;

                case nameof(string.Substring):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var startIndexType }], [var startIndexArgument]) when startIndexType.IsInt():
                            AnalyzeSubstring_Int32(consumer, element, invokedExpression, startIndexArgument);
                            break;
                    }
                    break;

                case nameof(string.ToString):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var formatProviderType }], [_]) when formatProviderType.IsIFormatProvider():
                            AnalyzeToString_IFormatProvider(consumer, element, invokedExpression);
                            break;
                    }
                    break;

                case nameof(string.Trim):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var trimCharsType }], var arguments) when trimCharsType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element):
                            AnalyzeTrim_CharArray(consumer, element, arguments);
                            break;
                    }
                    break;

                case nameof(string.TrimEnd):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var trimCharsType }], var arguments) when trimCharsType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element):
                            AnalyzeTrimEnd_CharArray(consumer, element, arguments);
                            break;
                    }
                    break;

                case nameof(string.TrimStart):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var trimCharsType }], var arguments) when trimCharsType.IsGenericArrayOf(PredefinedType.CHAR_FQN, element):
                            AnalyzeTrimStart_CharArray(consumer, element, arguments);
                            break;
                    }
                    break;
            }
        }
    }
}