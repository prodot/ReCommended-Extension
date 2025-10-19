using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;
using ReCommendedExtension.Extensions;
using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(UseExpressionResultSuggestion),
        typeof(UseStringListPatternSuggestion),
        typeof(UseOtherMethodSuggestion),
        typeof(UseStringPropertySuggestion),
        typeof(UseRangeIndexerSuggestion),
    ])]
public sealed class StringAnalyzer(NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
    : ElementProblemAnalyzer<IInvocationExpression>
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Underscore character used intentionally as a separator.")]
    static class Parameters
    {
        public static IReadOnlyList<Parameter> Char { get; } = [Parameter.Char];

        public static IReadOnlyList<Parameter> Char_Int32 { get; } = [Parameter.Char, Parameter.Int32];

        public static IReadOnlyList<Parameter> Char_StringComparison { get; } = [Parameter.Char, Parameter.StringComparison];

        public static IReadOnlyList<Parameter> Char_Int32_Int32 { get; } = [Parameter.Char, Parameter.Int32, Parameter.Int32];

        public static IReadOnlyList<Parameter> String { get; } = [Parameter.String];

        public static IReadOnlyList<Parameter> String_StringComparison { get; } = [Parameter.String, Parameter.StringComparison];
    }

    [Pure]
    static string CreateStringArray(string[] items, ICSharpExpression context)
    {
        if (context.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp120 && context.TryGetTargetType(true) is { })
        {
            return $"[{string.Join(", ", items)}]";
        }

        if (items is [])
        {
            if (PredefinedType.ARRAY_FQN.HasMethod(
                new MethodSignature { Name = nameof(Array.Empty), Parameters = [], GenericParametersCount = 1, IsStatic = true },
                context.GetPsiModule()))
            {
                return $"{nameof(Array)}.{nameof(Array.Empty)}<string>()";
            }

            return "new string[0]";
        }

        return $$"""new[] { {{string.Join(", ", items)}} }""";
    }

    [Pure]
    static string CreateConversionToString(string item, ICSharpExpression context)
    {
        if (context.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp60)
        {
            return $"$\"{{{item}}}\"";
        }

        return $"({item}).{nameof(ToString)}()";
    }

    /// <remarks>
    /// <c>text.Contains("")</c> → <c>true</c>
    /// </remarks>
    void AnalyzeContains_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (valueArgument.Value.TryGetStringConstant() == ""
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
        }
    }

    /// <remarks>
    /// <c>text.Contains("", StringComparison)</c> → <c>true</c>
    /// </remarks>
    void AnalyzeContains_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (valueArgument.Value.TryGetStringConstant() == ""
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always true.", invocationExpression, "true"));
        }
    }

    /// <remarks>
    /// <c>text.EndsWith(c)</c> → <c>text is [.., var lastChar] &amp;&amp; lastChar == c</c> (C# 11)<para/>
    /// <c>text.EndsWith('a')</c> → <c>text is [.., 'a']</c> (C# 11)
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
    /// <c>text.EndsWith("", comparisonType)</c> → <c>true</c><para/>
    /// <c>text.EndsWith("a", Ordinal)</c> → <c>text is [.., 'a']</c> (C# 11)<para/>
    /// <c>text.EndsWith("a", OrdinalIgnoresCase)</c> → <c>text is [.., 'a' or 'A']</c> (C# 11)
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
    /// <c>text.GetTypeCode()</c> → <c>TypeCode.String</c>
    /// </remarks>
    void AnalyzeGetTypeCode(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, IReferenceExpression invokedExpression)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {nameof(TypeCode)}.{nameof(TypeCode.String)}.",
                    invocationExpression,
                    $"{nameof(TypeCode)}.{nameof(TypeCode.String)}"));
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(c) == 0</c> → <c>text is [var firstChar, ..] &amp;&amp; firstChar == c</c> (C# 11)<para/>
    /// <c>text.IndexOf('a') == 0</c> → <c>text is ['a', ..]</c> (C# 11)<para/>
    /// <c>text.IndexOf(c) != 0</c> → <c>text is not [var firstChar, ..] || firstChar != c</c> (C# 11)<para/>
    /// <c>text.IndexOf('a') != 0</c> → <c>text is not ['a', ..]</c> (C# 11)<para/>
    /// <c>text.IndexOf(c) > -1</c> → <c>text.Contains(c)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(c) != -1</c> → <c>text.Contains(c) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(c) >= 0</c> → <c>text.Contains(c) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(c) == -1</c> → <c>!text.Contains(c) (.NET Core 2.1)</c><para/>
    /// <c>text.IndexOf(c) &lt; 0</c> → <c>!text.Contains(c) (.NET Core 2.1)</c>
    /// </remarks>
    void AnalyzeIndexOf_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        [Pure]
        bool MethodExists()
            => PredefinedType.STRING_FQN.HasMethod(
                new MethodSignature { Name = nameof(string.Contains), Parameters = Parameters.Char },
                invocationExpression.PsiModule);

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
                        && MethodExists():

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
                        && MethodExists():

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
                && valueArgument.Value is { }:

                var tokenType = relationalExpression.OperatorSign.GetTokenType();

                if ((tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0) && MethodExists())
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

                if (tokenType == CSharpTokenType.LT && value == 0 && MethodExists())
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
    /// <c>text.IndexOf(c, comparisonType) > -1</c> → <c>text.Contains(c, comparisonType)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(c, comparisonType) != -1</c> → <c>text.Contains(c, comparisonType)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(c, comparisonType) >= 0</c> → <c>text.Contains(c, comparisonType)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(c, comparisonType) == -1</c> → <c>!text.Contains(c, comparisonType)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(c, comparisonType) &lt; 0</c> → <c>!text.Contains(c, comparisonType)</c> (.NET Core 2.1)
    /// </remarks>
    void AnalyzeIndexOf_Char_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        [Pure]
        bool MethodExists()
            => PredefinedType.STRING_FQN.HasMethod(
                new MethodSignature { Name = nameof(string.Contains), Parameters = Parameters.Char_StringComparison },
                invocationExpression.PsiModule);

        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (invocationExpression.Parent)
        {
            case IEqualityExpression equalityExpression when equalityExpression.LeftOperand == invocationExpression
                && valueArgument.Value is { }
                && comparisonTypeArgument.Value is { }:

                switch (equalityExpression.EqualityType, equalityExpression.RightOperand.TryGetInt32Constant())
                {
                    case (EqualityExpressionType.NE, -1)
                        when invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
                        && MethodExists():

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
                        when invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
                        && MethodExists():

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

                if ((tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0) && MethodExists())
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

                if (tokenType == CSharpTokenType.LT && value == 0 && MethodExists())
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

    /// <remarks>
    /// <c>text.IndexOf("")</c> → <c>0</c><para/>
    /// <c>text.IndexOf(s) == 0</c> → <c>text.StartsWith(s)</c><para/>
    /// <c>text.IndexOf(s) != 0</c> → <c>!text.StartsWith(s)</c><para/>
    /// <c>text.IndexOf(s) > -1</c> → <c>text.Contains(s, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(s) != -1</c> → <c>text.Contains(s, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(s) >= 0</c> → <c>text.Contains(s, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(s) == -1</c> → <c>!text.Contains(s, CurrentCulture)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(s) &lt; 0</c> → <c>!text.Contains(s, CurrentCulture)</c> (.NET Core 2.1)
    /// </remarks>
    void AnalyzeIndexOf_String(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        [Pure]
        bool MethodStartsWithExists()
            => PredefinedType.STRING_FQN.HasMethod(
                new MethodSignature { Name = nameof(string.StartsWith), Parameters = Parameters.String },
                invocationExpression.PsiModule);

        [Pure]
        bool MethodContainsExists()
            => PredefinedType.STRING_FQN.HasMethod(
                new MethodSignature { Name = nameof(string.Contains), Parameters = Parameters.String_StringComparison },
                invocationExpression.PsiModule);

        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (valueArgument.Value.TryGetStringConstant() == ""
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always 0.", invocationExpression, "0"));
        }
        else
        {
            if (invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
            {
                switch (invocationExpression.Parent)
                {
                    case IEqualityExpression equalityExpression
                        when equalityExpression.LeftOperand == invocationExpression && valueArgument.Value is { }:

                        switch (equalityExpression.EqualityType, equalityExpression.RightOperand.TryGetInt32Constant())
                        {
                            case (EqualityExpressionType.EQEQ, 0) when MethodStartsWithExists():
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

                            case (EqualityExpressionType.NE, 0) when MethodStartsWithExists():
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

                            case (EqualityExpressionType.NE, -1) when MethodContainsExists():
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

                            case (EqualityExpressionType.EQEQ, -1) when MethodContainsExists():
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
                        && valueArgument.Value is { }:

                        var tokenType = relationalExpression.OperatorSign.GetTokenType();

                        if ((tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                            && MethodContainsExists())
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

                        if (tokenType == CSharpTokenType.LT && value == 0 && MethodContainsExists())
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
        }
    }

    /// <remarks>
    /// <c>text.IndexOf("", StringComparison)</c> → <c>0</c><para/>
    /// <c>text.IndexOf(s, comparisonType) == 0</c> → <c>text.StartsWith(s, comparisonType)</c><para/>
    /// <c>text.IndexOf(s, comparisonType) != 0</c> → <c>!text.StartsWith(s, comparisonType)</c><para/>
    /// <c>text.IndexOf(s, comparisonType) > -1</c> → <c>text.Contains(s, comparisonType)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(s, comparisonType) != -1</c> → <c>text.Contains(s, comparisonType)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(s, comparisonType) >= 0</c> → <c>text.Contains(s, comparisonType)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(s, comparisonType) == -1</c> → <c>!text.Contains(s, comparisonType)</c> (.NET Core 2.1)<para/>
    /// <c>text.IndexOf(s, comparisonType) &lt; 0</c> → <c>!text.Contains(s, comparisonType)</c> (.NET Core 2.1)
    /// </remarks>
    void AnalyzeIndexOf_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument comparisonTypeArgument)
    {
        [Pure]
        bool MethodStartsWithExists()
            => PredefinedType.STRING_FQN.HasMethod(
                new MethodSignature { Name = nameof(string.StartsWith), Parameters = Parameters.String_StringComparison },
                invocationExpression.PsiModule);

        [Pure]
        bool MethodContainsExists()
            => PredefinedType.STRING_FQN.HasMethod(
                new MethodSignature { Name = nameof(string.Contains), Parameters = Parameters.String_StringComparison },
                invocationExpression.PsiModule);

        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (valueArgument.Value.TryGetStringConstant() == ""
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always 0.", invocationExpression, "0"));
        }
        else
        {
            if (invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
            {
                switch (invocationExpression.Parent)
                {
                    case IEqualityExpression equalityExpression when equalityExpression.LeftOperand == invocationExpression
                        && valueArgument.Value is { }
                        && comparisonTypeArgument.Value is { }:

                        switch (equalityExpression.EqualityType, equalityExpression.RightOperand.TryGetInt32Constant())
                        {
                            case (EqualityExpressionType.EQEQ, 0) when MethodStartsWithExists():
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

                            case (EqualityExpressionType.NE, 0) when MethodStartsWithExists():
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

                            case (EqualityExpressionType.NE, -1) when MethodContainsExists():
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

                            case (EqualityExpressionType.EQEQ, -1) when MethodContainsExists():
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
                        && comparisonTypeArgument.Value is { }:

                        var tokenType = relationalExpression.OperatorSign.GetTokenType();

                        if ((tokenType == CSharpTokenType.GT && value == -1 || tokenType == CSharpTokenType.GE && value == 0)
                            && MethodContainsExists())
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

                        if (tokenType == CSharpTokenType.LT && value == 0 && MethodContainsExists())
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
    }

    /// <remarks>
    /// <c>text.IndexOfAny([])</c> → <c>-1</c><para/>
    /// <c>text.IndexOfAny([c])</c> → <c>text.IndexOf(c)</c>
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

            case { Count: 1 } collectionCreation when PredefinedType.STRING_FQN.HasMethod(
                new MethodSignature { Name = nameof(string.IndexOf), Parameters = Parameters.Char },
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherMethodSuggestion(
                        $"Use the '{nameof(string.IndexOf)}' method.",
                        invocationExpression,
                        invokedExpression,
                        nameof(string.IndexOf),
                        false,
                        [collectionCreation.SingleElement.GetText()]));
                break;
        }
    }

    /// <remarks>
    /// <c>text.IndexOfAny([c], startIndex)</c> → <c>text.IndexOf(c, startIndex)</c>
    /// </remarks>
    static void AnalyzeIndexOfAny_CharArray_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument anyOfArgument,
        ICSharpArgument startIndexArgument)
    {
        if (CollectionCreation.TryFrom(anyOfArgument.Value) is { Count: 1 } collectionCreation
            && startIndexArgument.Value is { }
            && PredefinedType.STRING_FQN.HasMethod(
                new MethodSignature { Name = nameof(string.IndexOf), Parameters = Parameters.Char_Int32 },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new UseOtherMethodSuggestion(
                    $"Use the '{nameof(string.IndexOf)}' method.",
                    invocationExpression,
                    invokedExpression,
                    nameof(string.IndexOf),
                    false,
                    [collectionCreation.SingleElement.GetText(), startIndexArgument.Value.GetText()]));
        }
    }

    /// <remarks>
    /// <c>text.IndexOfAny([c], startIndex, count)</c> → <c>text.IndexOf(c, startIndex, count)</c>
    /// </remarks>
    static void AnalyzeIndexOfAny_CharArray_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument anyOfArgument,
        ICSharpArgument startIndexArgument,
        ICSharpArgument countArgument)
    {
        if (CollectionCreation.TryFrom(anyOfArgument.Value) is { Count: 1 } collectionCreation
            && startIndexArgument.Value is { }
            && countArgument.Value is { }
            && PredefinedType.STRING_FQN.HasMethod(
                new MethodSignature { Name = nameof(string.IndexOf), Parameters = Parameters.Char_Int32_Int32 },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new UseOtherMethodSuggestion(
                    $"Use the '{nameof(string.IndexOf)}' method.",
                    invocationExpression,
                    invokedExpression,
                    nameof(string.IndexOf),
                    false,
                    [collectionCreation.SingleElement.GetText(), startIndexArgument.Value.GetText(), countArgument.Value.GetText()]));
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, [])</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item])</c> → <c>$"{item}"</c>
    /// </remarks>
    static void AnalyzeJoin_String_ObjectArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        if (arguments.TrySplit(out _, out var valuesArguments) && !invocationExpression.IsUsedAsStatement())
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    return;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                                return;

                            case 1:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion(
                                        "The expression is always the same as the passed element converted to string.",
                                        invocationExpression,
                                        CreateConversionToString(collectionCreation.SingleElement.GetText(), invocationExpression)));
                                return;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsGenericArrayOfObject())
                        {
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always the same as the passed element converted to string.",
                                    invocationExpression,
                                    CreateConversionToString(argument.Value.GetText(), invocationExpression)));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, default(ReadOnlySpan&lt;object?&gt;))</c> → <c>""</c><para/>
    /// <c>string.Join(separator, new ReadOnlySpan&lt;object?&gt;())</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item])</c> → <c>$"{item}"</c><para/>
    /// <c>string.Join(separator, item)</c> → <c>$"{item}"</c>
    /// </remarks>
    static void AnalyzeJoin_String_ReadOnlySpanOfObject(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        if (arguments.TrySplit(out _, out var valuesArguments) && !invocationExpression.IsUsedAsStatement())
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    return;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                                return;

                            case 1:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion(
                                        "The expression is always the same as the passed element converted to string.",
                                        invocationExpression,
                                        CreateConversionToString(collectionCreation.SingleElement.GetText(), invocationExpression)));
                                return;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsReadOnlySpanOfObject())
                        {
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always the same as the passed element converted to string.",
                                    invocationExpression,
                                    CreateConversionToString(argument.Value.GetText(), invocationExpression)));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, [])</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item])</c> → <c>$"{item}"</c>
    /// </remarks>
    static void AnalyzeJoin_String_IEnumerableOfT(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valuesArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && CollectionCreation.TryFrom(valuesArgument.Value) is { } collectionCreation)
        {
            switch (collectionCreation.Count)
            {
                case 0:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    return;

                case 1:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always the same as the passed element converted to string.",
                            invocationExpression,
                            CreateConversionToString(collectionCreation.SingleElement.GetText(), invocationExpression)));
                    return;
            }
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, [])</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item])</c> → <c>item</c>
    /// </remarks>
    static void AnalyzeJoin_String_StringArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        if (arguments.TrySplit(out _, out var valuesArguments) && !invocationExpression.IsUsedAsStatement())
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    return;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                                return;

                            case 1:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion(
                                        "The expression is always the same as the passed element.",
                                        invocationExpression,
                                        collectionCreation.SingleElement.GetText()));
                                return;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsGenericArrayOfString())
                        {
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always the same as the passed element.",
                                    invocationExpression,
                                    argument.Value.GetText()));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, values, 0, 0)</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item], 1, 0)</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item], 0, 1)</c> → <c>item</c>
    /// </remarks>
    static void AnalyzeJoin_String_StringArray_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valuesArgument,
        ICSharpArgument startIndexArgument,
        ICSharpArgument countArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            switch (CollectionCreation.TryFrom(valuesArgument.Value), startIndexArgument.Value.TryGetInt32Constant(),
                countArgument.Value.TryGetInt32Constant())
            {
                case (_, 0, 0) or ({ Count: 1 }, 1, 0):
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    return;

                case ({ Count: 1 } collectionCreation, 0, 1):
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always the same as the passed element.",
                            invocationExpression,
                            collectionCreation.SingleElement.GetText()));
                    return;
            }
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, default(ReadOnlySpan&lt;string?&gt;))</c> → <c>""</c><para/>
    /// <c>string.Join(separator, new ReadOnlySpan&lt;string?&gt;())</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item])</c> → <c>item</c><para/>
    /// <c>string.Join(separator, item)</c> → <c>item</c>
    /// </remarks>
    static void AnalyzeJoin_String_ReadOnlySpanOfString(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        if (arguments.TrySplit(out _, out var valuesArguments) && !invocationExpression.IsUsedAsStatement())
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    return;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                                return;

                            case 1:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion(
                                        "The expression is always the same as the passed element.",
                                        invocationExpression,
                                        collectionCreation.SingleElement.GetText()));
                                return;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsReadOnlySpanOfString())
                        {
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always the same as the passed element.",
                                    invocationExpression,
                                    argument.Value.GetText()));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, [])</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item])</c> → <c>$"{item}"</c>
    /// </remarks>
    static void AnalyzeJoin_Char_ObjectArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        if (arguments.TrySplit(out _, out var valuesArguments) && !invocationExpression.IsUsedAsStatement())
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    break;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                                break;

                            case 1:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion(
                                        "The expression is always the same as the passed element converted to string.",
                                        invocationExpression,
                                        CreateConversionToString(collectionCreation.SingleElement.GetText(), invocationExpression)));
                                break;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsGenericArrayOfObject())
                        {
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always the same as the passed element converted to string.",
                                    invocationExpression,
                                    CreateConversionToString(argument.Value.GetText(), invocationExpression)));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, default(ReadOnlySpan&lt;object?&gt;))</c> → <c>""</c><para/>
    /// <c>string.Join(separator, new ReadOnlySpan&lt;object?&gt;())</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item])</c> → <c>$"{item}"</c><para/>
    /// <c>string.Join(separator, item)</c> → <c>$"{item}"</c>
    /// </remarks>
    static void AnalyzeJoin_Char_ReadOnlySpanOfObject(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        if (arguments.TrySplit(out _, out var valuesArguments) && !invocationExpression.IsUsedAsStatement())
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    break;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                                break;

                            case 1:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion(
                                        "The expression is always the same as the passed element converted to string.",
                                        invocationExpression,
                                        CreateConversionToString(collectionCreation.SingleElement.GetText(), invocationExpression)));
                                break;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsReadOnlySpanOfObject())
                        {
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always the same as the passed element converted to string.",
                                    invocationExpression,
                                    CreateConversionToString(argument.Value.GetText(), invocationExpression)));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, [])</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item])</c> → <c>$"{item}"</c>
    /// </remarks>
    static void AnalyzeJoin_Char_IEnumerableOfT(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valuesArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && CollectionCreation.TryFrom(valuesArgument.Value) is { } collectionCreation)
        {
            switch (collectionCreation.Count)
            {
                case 0:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    break;

                case 1:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always the same as the passed element converted to string.",
                            invocationExpression,
                            CreateConversionToString(collectionCreation.SingleElement.GetText(), invocationExpression)));
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, [])</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item])</c> → <c>item</c>
    /// </remarks>
    static void AnalyzeJoin_Char_StringArray(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        if (arguments.TrySplit(out _, out var valuesArguments) && !invocationExpression.IsUsedAsStatement())
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    break;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                                break;

                            case 1:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion(
                                        "The expression is always the same as the passed element.",
                                        invocationExpression,
                                        collectionCreation.SingleElement.GetText()));
                                break;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsGenericArrayOfString())
                        {
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always the same as the passed element.",
                                    invocationExpression,
                                    argument.Value.GetText()));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, values, 0, 0)</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item], 1, 0)</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item], 0, 1)</c> → <c>item</c>
    /// </remarks>
    static void AnalyzeJoin_Char_StringArray_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valuesArgument,
        ICSharpArgument startIndexArgument,
        ICSharpArgument countArgument)
    {
        if (!invocationExpression.IsUsedAsStatement())
        {
            switch (CollectionCreation.TryFrom(valuesArgument.Value), startIndexArgument.Value.TryGetInt32Constant(),
                countArgument.Value.TryGetInt32Constant())
            {
                case (_, 0, 0) or ({ Count: 1 }, 1, 0):
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    break;

                case ({ Count: 1 } collectionCreation, 0, 1):
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always the same as the passed element.",
                            invocationExpression,
                            collectionCreation.SingleElement.GetText()));
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>string.Join(separator, default(ReadOnlySpan&lt;string?&gt;))</c> → <c>""</c><para/>
    /// <c>string.Join(separator, new ReadOnlySpan&lt;string?&gt;())</c> → <c>""</c><para/>
    /// <c>string.Join(separator, [item])</c> → <c>item</c><para/>
    /// <c>string.Join(separator, item)</c> → <c>item</c>
    /// </remarks>
    static void AnalyzeJoin_Char_ReadOnlySpanOfString(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument?> arguments)
    {
        if (arguments.TrySplit(out _, out var valuesArguments) && !invocationExpression.IsUsedAsStatement())
        {
            switch (valuesArguments)
            {
                case []:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                    break;

                case [{ Value: { } } argument]:
                    if (CollectionCreation.TryFrom(argument.Value) is { } collectionCreation)
                    {
                        switch (collectionCreation.Count)
                        {
                            case 0:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion("The expression is always an empty string.", invocationExpression, "\"\""));
                                break;

                            case 1:
                                consumer.AddHighlighting(
                                    new UseExpressionResultSuggestion(
                                        "The expression is always the same as the passed element.",
                                        invocationExpression,
                                        collectionCreation.SingleElement.GetText()));
                                break;
                        }
                    }
                    else
                    {
                        if (argument.Value.GetExpressionType().ToIType() is { } argumentType && !argumentType.IsReadOnlySpanOfString())
                        {
                            consumer.AddHighlighting(
                                new UseExpressionResultSuggestion(
                                    "The expression is always the same as the passed element.",
                                    invocationExpression,
                                    argument.Value.GetText()));
                        }
                    }
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>text.LastIndexOf(c, 0)</c> → <c>-1</c>
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
    /// <c>text.LastIndexOf("", comparisonType)</c> → <c>text.Length</c> (.NET 5)
    /// </remarks>
    static void AnalyzeLastIndexOf_String_StringComparison(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        if (valueArgument.Value.TryGetStringConstant() == ""
            && invocationExpression.PsiModule.TargetFrameworkId.Version.Major >= 5
            && !invocationExpression.IsUsedAsStatement())
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
    /// <c>text.LastIndexOfAny([])</c> → <c>-1</c><para/>
    /// <c>text.LastIndexOfAny([c])</c> → <c>text.LastIndexOf(c)</c>
    /// </remarks>
    void AnalyzeLastIndexOfAny_CharArray(
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

            case { Count: 1 } collectionCreation when PredefinedType.STRING_FQN.HasMethod(
                new MethodSignature { Name = nameof(string.LastIndexOf), Parameters = Parameters.Char },
                invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherMethodSuggestion(
                        $"Use the '{nameof(string.LastIndexOf)}' method.",
                        invocationExpression,
                        invokedExpression,
                        nameof(string.LastIndexOf),
                        false,
                        [collectionCreation.SingleElement.GetText()]));
                break;
        }
    }

    /// <remarks>
    /// <c>text.LastIndexOfAny(chars, 0)</c> → <c>-1</c><para/>
    /// <c>text.LastIndexOfAny([c], startIndex)</c> → <c>text.LastIndexOf(c, startIndex)</c>
    /// </remarks>
    void AnalyzeLastIndexOfAny_CharArray_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument anyOfArgument,
        ICSharpArgument startIndexArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (startIndexArgument.Value.TryGetInt32Constant() == 0
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always -1.", invocationExpression, "-1"));
            return;
        }

        switch (CollectionCreation.TryFrom(anyOfArgument.Value))
        {
            case { Count: 1 } collectionCreation when startIndexArgument.Value is { }
                && PredefinedType.STRING_FQN.HasMethod(
                    new MethodSignature { Name = nameof(string.LastIndexOf), Parameters = Parameters.Char_Int32 },
                    invocationExpression.PsiModule):

                consumer.AddHighlighting(
                    new UseOtherMethodSuggestion(
                        $"Use the '{nameof(string.LastIndexOf)}' method.",
                        invocationExpression,
                        invokedExpression,
                        nameof(string.LastIndexOf),
                        false,
                        [collectionCreation.SingleElement.GetText(), startIndexArgument.Value.GetText()]));
                break;
        }
    }

    /// <remarks>
    /// <c>text.LastIndexOfAny([c], 0, 0)</c> → <c>-1</c><para/>
    /// <c>text.LastIndexOfAny([c], 0, 1)</c> → <c>-1</c><para/>
    /// <c>text.LastIndexOfAny([c], startIndex, count)</c> → <c>text.LastIndexOf(c, startIndex, count)</c>
    /// </remarks>
    void AnalyzeLastIndexOfAny_CharArray_Int32_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument anyOfArgument,
        ICSharpArgument startIndexArgument,
        ICSharpArgument countArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (startIndexArgument.Value.TryGetInt32Constant() == 0
            && countArgument.Value.TryGetInt32Constant() is 0 or 1
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always -1.", invocationExpression, "-1"));
            return;
        }

        if (CollectionCreation.TryFrom(anyOfArgument.Value) is { Count: 1 } collectionCreation
            && startIndexArgument.Value is { }
            && countArgument.Value is { }
            && PredefinedType.STRING_FQN.HasMethod(
                new MethodSignature { Name = nameof(string.LastIndexOf), Parameters = Parameters.Char_Int32_Int32 },
                invocationExpression.PsiModule))
        {
            consumer.AddHighlighting(
                new UseOtherMethodSuggestion(
                    $"Use the '{nameof(string.LastIndexOf)}' method.",
                    invocationExpression,
                    invokedExpression,
                    nameof(string.LastIndexOf),
                    false,
                    [collectionCreation.SingleElement.GetText(), startIndexArgument.Value.GetText(), countArgument.Value.GetText()]));
        }
    }

    /// <remarks>
    /// <c>text.Remove(0)</c> → <c>""</c> (.NET 6)<para/>
    /// <c>text.Remove(startIndex)</c> → <c>text[..startIndex]</c> (C# 8)
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
    /// <c>text.Remove(0, count)</c> → <c>text[count..]</c> (C# 8)
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
    /// <c>text.Split(separator, 0, StringSplitOptions)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(separator, 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(separator, 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)
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

                        case (StringSplitOptions)2 when PredefinedType.STRING_FQN.HasMethod(
                            new MethodSignature { Name = nameof(string.Trim), Parameters = [] },
                            invocationExpression.PsiModule): // todo: use StringSplitOptions.TrimEntries

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
    /// <c>text.Split(separator, 0)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(separator, 1)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)
    /// </remarks>
    void AnalyzeSplit_CharArray_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument countArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (countArgument.Value.TryGetInt32Constant())
        {
            case 0 when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateStringArray([], invocationExpression)));
                break;

            case 1 when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                Debug.Assert(invokedExpression.QualifierExpression is { });

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an array with a single element.",
                        invocationExpression,
                        CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                break;
        }
    }

    /// <remarks>
    /// <c>text.Split(separator, 0, options)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(separator, 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(separator, 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)
    /// </remarks>
    void AnalyzeSplit_CharArray_Int32_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument countArgument,
        ICSharpArgument optionsArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        switch (countArgument.Value.TryGetInt32Constant())
        {
            case 0 when !invocationExpression.IsUsedAsStatement()
                && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer):

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion(
                        "The expression is always an empty array.",
                        invocationExpression,
                        CreateStringArray([], invocationExpression)));
                break;

            case 1 when !invocationExpression.IsUsedAsStatement()
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

                    case (StringSplitOptions)2 when PredefinedType.STRING_FQN.HasMethod(
                        new MethodSignature { Name = nameof(string.Trim), Parameters = [] },
                        invocationExpression.PsiModule): // todo: use StringSplitOptions.TrimEntries

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

    /// <remarks>
    /// <c>text.Split(null, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split("", None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(null, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split("", TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)
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

        if (separator == ""
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
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

                case (StringSplitOptions)2
                    when PredefinedType.STRING_FQN.HasMethod(
                        new MethodSignature { Name = nameof(string.Trim), Parameters = [] },
                        invocationExpression.PsiModule): // todo: use StringSplitOptions.TrimEntries
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always an array with a single trimmed element.",
                            invocationExpression,
                            CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(separator, 0, StringSplitOptions)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(separator, 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(separator, 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split(null, count, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split("", count, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(null, count, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split("", count, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)
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

                    case (StringSplitOptions)2 when PredefinedType.STRING_FQN.HasMethod(
                        new MethodSignature { Name = nameof(string.Trim), Parameters = [] },
                        invocationExpression.PsiModule): // todo: use StringSplitOptions.TrimEntries

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

    /// <remarks>
    /// <c>text.Split([""], None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split([""], TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)
    /// </remarks>
    void AnalyzeSplit_StringArray_StringSplitOptions(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument separatorArgument,
        ICSharpArgument optionsArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (CollectionCreation.TryFrom(separatorArgument.Value) is { Count: > 0, StringConstants: [""] }
            && !invocationExpression.IsUsedAsStatement()
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer))
        {
            switch (optionsArgument.Value.TryGetStringSplitOptionsConstant())
            {
                case StringSplitOptions.None:
                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always an array with a single element.",
                            invocationExpression,
                            CreateStringArray([invokedExpression.QualifierExpression.GetText()], invocationExpression)));
                    break;

                case (StringSplitOptions)2 when PredefinedType.STRING_FQN.HasMethod(
                    new MethodSignature { Name = nameof(string.Trim), Parameters = [] },
                    invocationExpression.PsiModule): // todo: use StringSplitOptions.TrimEntries

                    consumer.AddHighlighting(
                        new UseExpressionResultSuggestion(
                            "The expression is always an array with a single trimmed element.",
                            invocationExpression,
                            CreateStringArray([$"{invokedExpression.QualifierExpression.GetText()}.Trim()"], invocationExpression)));
                    break;
            }
        }
    }

    /// <remarks>
    /// <c>text.Split(separator, 0, StringSplitOptions)</c> → <c>Array.Empty&lt;string&gt;()</c> or <c>[]</c> (C# 12)<para/>
    /// <c>text.Split(separator, 1, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split(separator, 1, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)<para/>
    /// <c>text.Split([""], count, None)</c> → <c>new[] { text }</c> or <c>[text]</c> (C# 12)<para/>
    /// <c>text.Split([""], count, TrimEntries)</c> → <c>new[] { text.Trim() }</c> or <c>[text.Trim()]</c> (C# 12)
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
        }
    }

    /// <remarks>
    /// <c>text.StartsWith(c)</c> → <c>text is [var firstChar, ..] &amp;&amp; firstChar == c</c> (C# 11)<para/>
    /// <c>text.StartsWith('a')</c> → <c>text is ['a', ..]</c> (C# 11)
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
    /// <c>text.StartsWith("", comparisonType)</c> → <c>true</c><para/>
    /// <c>text.StartsWith("a", Ordinal)</c> → <c>text is ['a', ..]</c> (C# 11)<para/>
    /// <c>text.StartsWith("a", OrdinalIgnoresCase)</c> → <c>text is ['a' or 'A', ..]</c> (C# 11)
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

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
            } method
            && method.ContainingType.IsSystemString())
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false, TypeParameters: [] }):
                    switch (method.ShortName)
                    {
                        case nameof(string.Contains):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsString():
                                    AnalyzeContains_String(consumer, element, invokedExpression, valueArgument);
                                    break;

                                case ([{ Type: var valueType }, { Type: var stringComparisonType }], [{ } valueArgument, _])
                                    when valueType.IsString() && stringComparisonType.IsStringComparison():

                                    AnalyzeContains_String_StringComparison(consumer, element, invokedExpression, valueArgument);
                                    break;
                            }
                            break;

                        case nameof(string.EndsWith):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsChar():
                                    AnalyzeEndsWith_Char(consumer, element, invokedExpression, valueArgument);
                                    break;

                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsString():
                                    AnalyzeEndsWith_String(consumer, element, invokedExpression, valueArgument);
                                    break;

                                case ([{ Type: var valueType }, { Type: var stringComparisonType }], [{ } valueArgument, { } comparisonTypeArgument])
                                    when valueType.IsString() && stringComparisonType.IsStringComparison():

                                    AnalyzeEndsWith_String_StringComparison(
                                        consumer,
                                        element,
                                        invokedExpression,
                                        valueArgument,
                                        comparisonTypeArgument);
                                    break;
                            }
                            break;

                        case nameof(string.GetTypeCode):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([], []): AnalyzeGetTypeCode(consumer, element, invokedExpression); break;
                            }
                            break;

                        case nameof(string.IndexOf):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsChar():
                                    AnalyzeIndexOf_Char(consumer, element, invokedExpression, valueArgument);
                                    break;

                                case ([{ Type: var valueType }, { Type: var stringComparisonType }], [{ } valueArgument, { } comparisonTypeArgument])
                                    when valueType.IsChar() && stringComparisonType.IsStringComparison():

                                    AnalyzeIndexOf_Char_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                                    break;

                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsString():
                                    AnalyzeIndexOf_String(consumer, element, invokedExpression, valueArgument);
                                    break;

                                case ([{ Type: var valueType }, { Type: var stringComparisonType }], [{ } valueArgument, { } comparisonTypeArgument])
                                    when valueType.IsString() && stringComparisonType.IsStringComparison():

                                    AnalyzeIndexOf_String_StringComparison(consumer, element, invokedExpression, valueArgument, comparisonTypeArgument);
                                    break;
                            }
                            break;

                        case nameof(string.IndexOfAny):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var anyOfType }], [{ } anyOfArgument]) when anyOfType.IsGenericArrayOfChar():
                                    AnalyzeIndexOfAny_CharArray(consumer, element, invokedExpression, anyOfArgument);
                                    break;

                                case ([{ Type: var anyOfType }, { Type: var startIndexType }], [{ } anyOfArgument, { } startIndexArgument])
                                    when anyOfType.IsGenericArrayOfChar() && startIndexType.IsInt():

                                    AnalyzeIndexOfAny_CharArray_Int32(consumer, element, invokedExpression, anyOfArgument, startIndexArgument);
                                    break;

                                case ([{ Type: var anyOfType }, { Type: var startIndexType }, { Type: var countType }], [
                                    { } anyOfArgument, { } startIndexArgument, { } valueArgument,
                                ]) when anyOfType.IsGenericArrayOfChar() && startIndexType.IsInt() && countType.IsInt():

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
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }, { Type: var startIndexType }], [_, { } startIndexArgument])
                                    when valueType.IsChar() && startIndexType.IsInt():

                                    AnalyzeLastIndexOf_Char_Int32(consumer, element, invokedExpression, startIndexArgument);
                                    break;

                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsString():
                                    AnalyzeLastIndexOf_String(consumer, element, invokedExpression, valueArgument);
                                    break;

                                case ([{ Type: var valueType }, { Type: var stringComparisonType }], [{ } valueArgument, _])
                                    when valueType.IsString() && stringComparisonType.IsStringComparison():

                                    AnalyzeLastIndexOf_String_StringComparison(consumer, element, invokedExpression, valueArgument);
                                    break;
                            }
                            break;

                        case nameof(string.LastIndexOfAny):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var anyOfType }], [{ } anyOfArgument]) when anyOfType.IsGenericArrayOfChar():
                                    AnalyzeLastIndexOfAny_CharArray(consumer, element, invokedExpression, anyOfArgument);
                                    break;

                                case ([{ Type: var anyOfType }, { Type: var startIndexType }], [{ } anyOfArgument, { } startIndexArgument])
                                    when anyOfType.IsGenericArrayOfChar() && startIndexType.IsInt():

                                    AnalyzeLastIndexOfAny_CharArray_Int32(consumer, element, invokedExpression, anyOfArgument, startIndexArgument);
                                    break;

                                case ([{ Type: var anyOfType }, { Type: var startIndexType }, { Type: var countType }], [
                                    { } anyOfArgument, { } startIndexArgument, { } valueArgument,
                                ]) when anyOfType.IsGenericArrayOfChar() && startIndexType.IsInt() && countType.IsInt():
                                    AnalyzeLastIndexOfAny_CharArray_Int32_Int32(
                                        consumer,
                                        element,
                                        invokedExpression,
                                        anyOfArgument,
                                        startIndexArgument,
                                        valueArgument);
                                    break;
                            }
                            break;

                        case nameof(string.Remove):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var startIndexType }], [{ } startIndexArgument]) when startIndexType.IsInt():
                                    AnalyzeRemove_Int32(consumer, element, invokedExpression, startIndexArgument);
                                    break;

                                case ([{ Type: var startIndexType }, { Type: var countType }], [{ } startIndexArgument, { } countArgument])
                                    when startIndexType.IsInt() && countType.IsInt():

                                    AnalyzeRemove_Int32_Int32(consumer, element, invokedExpression, startIndexArgument, countArgument);
                                    break;
                            }
                            break;

                        case nameof(string.Split):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], [
                                    _, { } countArgument, var optionsArgument,
                                ]) when separatorType.IsChar() && countType.IsInt() && optionsType.IsStringSplitOptions():

                                    AnalyzeSplit_Char_Int32_StringSplitOptions(consumer, element, invokedExpression, countArgument, optionsArgument);
                                    break;

                                case ([{ Type: var separatorType }, { Type: var countType }], [_, { } countArgument])
                                    when separatorType.IsGenericArrayOfChar() && countType.IsInt():

                                    AnalyzeSplit_CharArray_Int32(consumer, element, invokedExpression, countArgument);
                                    break;

                                case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], [
                                    _, { } countArgument, { } optionsArgument,
                                ]) when separatorType.IsGenericArrayOfChar() && countType.IsInt() && optionsType.IsStringSplitOptions():
                                    AnalyzeSplit_CharArray_Int32_StringSplitOptions(
                                        consumer,
                                        element,
                                        invokedExpression,
                                        countArgument,
                                        optionsArgument);
                                    break;

                                case ([{ Type: var separatorType }, { Type: var optionsType }], [{ } separatorArgument, var optionsArgument])
                                    when separatorType.IsString() && optionsType.IsStringSplitOptions():

                                    AnalyzeSplit_String_StringSplitOptions(consumer, element, invokedExpression, separatorArgument, optionsArgument);
                                    break;

                                case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], [
                                    { } separatorArgument, { } countArgument, var optionsArgument,
                                ]) when separatorType.IsString() && countType.IsInt() && optionsType.IsStringSplitOptions():

                                    AnalyzeSplit_String_Int32_StringSplitOptions(
                                        consumer,
                                        element,
                                        invokedExpression,
                                        separatorArgument,
                                        countArgument,
                                        optionsArgument);
                                    break;

                                case ([{ Type: var separatorType }, { Type: var optionsType }], [{ } separatorArgument, { } optionsArgument])
                                    when separatorType.IsGenericArrayOfString() && optionsType.IsStringSplitOptions():

                                    AnalyzeSplit_StringArray_StringSplitOptions(consumer, element, invokedExpression, separatorArgument, optionsArgument);
                                    break;

                                case ([{ Type: var separatorType }, { Type: var countType }, { Type: var optionsType }], [
                                    { } separatorArgument, { } countArgument, { } optionsArgument,
                                ]) when separatorType.IsGenericArrayOfString() && countType.IsInt() && optionsType.IsStringSplitOptions():
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
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsChar():
                                    AnalyzeStartsWith_Char(consumer, element, invokedExpression, valueArgument);
                                    break;

                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsString():
                                    AnalyzeStartsWith_String(consumer, element, invokedExpression, valueArgument);
                                    break;

                                case ([{ Type: var valueType }, { Type: var stringComparisonType }], [{ } valueArgument, { } comparisonTypeArgument])
                                    when valueType.IsString() && stringComparisonType.IsStringComparison():

                                    AnalyzeStartsWith_String_StringComparison(
                                        consumer,
                                        element,
                                        invokedExpression,
                                        valueArgument,
                                        comparisonTypeArgument);
                                    break;
                            }
                            break;
                    }
                    break;

                case (_, { IsStatic: true }):
                    switch (method.ShortName)
                    {
                        case nameof(string.Join):
                            switch (method.TypeParameters, method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                                    when separatorType.IsString() && valuesType.IsGenericArrayOfObject():

                                    AnalyzeJoin_String_ObjectArray(consumer, element, arguments);
                                    break;

                                case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                                    when separatorType.IsString() && valuesType.IsReadOnlySpanOfObject():

                                    AnalyzeJoin_String_ReadOnlySpanOfObject(consumer, element, arguments);
                                    break;

                                case ([_], [{ Type: var separatorType }, { Type: var valuesType }], [_, { } valuesArgument])
                                    when separatorType.IsString() && valuesType.IsGenericIEnumerable():

                                    AnalyzeJoin_String_IEnumerableOfT(consumer, element, valuesArgument);
                                    break;

                                case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                                    when separatorType.IsString() && valuesType.IsGenericArrayOfString():

                                    AnalyzeJoin_String_StringArray(consumer, element, arguments);
                                    break;

                                case ([], [
                                        { Type: var separatorType }, { Type: var valuesType }, { Type: var startIndexType }, { Type: var countType },
                                    ], [_, { } valuesArgument, { } startIndexArgument, { } countArgument]) when separatorType.IsString()
                                    && valuesType.IsGenericArrayOfString()
                                    && startIndexType.IsInt()
                                    && countType.IsInt():

                                    AnalyzeJoin_String_StringArray_Int32_Int32(consumer, element, valuesArgument, startIndexArgument, countArgument);
                                    break;

                                case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                                    when separatorType.IsString() && valuesType.IsReadOnlySpanOfString():

                                    AnalyzeJoin_String_ReadOnlySpanOfString(consumer, element, arguments);
                                    break;

                                case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                                    when separatorType.IsChar() && valuesType.IsGenericArrayOfObject():

                                    AnalyzeJoin_Char_ObjectArray(consumer, element, arguments);
                                    break;

                                case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                                    when separatorType.IsChar() && valuesType.IsReadOnlySpanOfObject():

                                    AnalyzeJoin_Char_ReadOnlySpanOfObject(consumer, element, arguments);
                                    break;

                                case ([_], [{ Type: var separatorType }, { Type: var valuesType }], [_, { } valuesArgument])
                                    when separatorType.IsChar() && valuesType.IsGenericIEnumerable():

                                    AnalyzeJoin_Char_IEnumerableOfT(consumer, element, valuesArgument);
                                    break;

                                case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                                    when separatorType.IsChar() && valuesType.IsGenericArrayOfString():

                                    AnalyzeJoin_Char_StringArray(consumer, element, arguments);
                                    break;

                                case ([], [
                                        { Type: var separatorType }, { Type: var valuesType }, { Type: var startIndexType }, { Type: var countType },
                                    ], [_, { } valuesArgument, { } startIndexArgument, { } countArgument]) when separatorType.IsChar()
                                    && valuesType.IsGenericArrayOfString()
                                    && startIndexType.IsInt()
                                    && countType.IsInt():

                                    AnalyzeJoin_Char_StringArray_Int32_Int32(consumer, element, valuesArgument, startIndexArgument, countArgument);
                                    break;

                                case ([], [{ Type: var separatorType }, { Type: var valuesType }], { } arguments)
                                    when separatorType.IsChar() && valuesType.IsReadOnlySpanOfString():

                                    AnalyzeJoin_Char_ReadOnlySpanOfString(consumer, element, arguments);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}