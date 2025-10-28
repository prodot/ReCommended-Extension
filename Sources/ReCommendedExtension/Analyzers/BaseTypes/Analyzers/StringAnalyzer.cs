using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl.ControlFlow.NullableAnalysis.Runner;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseStringListPatternSuggestion), typeof(UseStringPropertySuggestion), typeof(UseRangeIndexerSuggestion)])]
public sealed class StringAnalyzer(NullableReferenceTypesDataFlowAnalysisRunSynchronizer nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
    : ElementProblemAnalyzer<IInvocationExpression>
{
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
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
            && valueArgument.Value.TryGetStringConstant() is [var character]
            && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110)
        {
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
        }
    }

    /// <remarks>
    /// <c>text.IndexOf(c) == 0</c> → <c>text is [var firstChar, ..] &amp;&amp; firstChar == c</c> (C# 11)<para/>
    /// <c>text.IndexOf('a') == 0</c> → <c>text is ['a', ..]</c> (C# 11)<para/>
    /// <c>text.IndexOf(c) != 0</c> → <c>text is not [var firstChar, ..] || firstChar != c</c> (C# 11)<para/>
    /// <c>text.IndexOf('a') != 0</c> → <c>text is not ['a', ..]</c> (C# 11)
    /// </remarks>
    static void AnalyzeIndexOf_Char(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (invocationExpression.Parent is IEqualityExpression equalityExpression
            && equalityExpression.LeftOperand == invocationExpression
            && valueArgument.Value is { })
        {
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
            }
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
    /// <c>text.Remove(startIndex)</c> → <c>text[..startIndex]</c> (C# 8)
    /// </remarks>
    static void AnalyzeRemove_Int32(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument startIndexArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement()
            && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp80
            && startIndexArgument.Value.TryGetInt32Constant() is null or > 0
            && startIndexArgument.Value is { })
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
            && invokedExpression.QualifierExpression.IsNotNullHere(nullableReferenceTypesDataFlowAnalysisRunSynchronizer)
            && valueArgument.Value.TryGetStringConstant() is [var character]
            && invocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110)
        {
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
                        case nameof(string.EndsWith):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsChar():
                                    AnalyzeEndsWith_Char(consumer, element, invokedExpression, valueArgument);
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

                        case nameof(string.IndexOf):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsChar():
                                    AnalyzeIndexOf_Char(consumer, element, invokedExpression, valueArgument);
                                    break;
                            }
                            break;

                        case nameof(string.LastIndexOf):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsString():
                                    AnalyzeLastIndexOf_String(consumer, element, invokedExpression, valueArgument);
                                    break;

                                case ([{ Type: var valueType }, { Type: var stringComparisonType }], [{ } valueArgument, _])
                                    when valueType.IsString() && stringComparisonType.IsStringComparison():

                                    AnalyzeLastIndexOf_String_StringComparison(consumer, element, invokedExpression, valueArgument);
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

                        case nameof(string.StartsWith):
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsChar():
                                    AnalyzeStartsWith_Char(consumer, element, invokedExpression, valueArgument);
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
            }
        }
    }
}