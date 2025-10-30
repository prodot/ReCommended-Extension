using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(typeof(IInvocationExpression), HighlightingTypes = [typeof(UseStringPropertySuggestion), typeof(UseRangeIndexerSuggestion)])]
public sealed class StringAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
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
                    }
                    break;
            }
        }
    }
}