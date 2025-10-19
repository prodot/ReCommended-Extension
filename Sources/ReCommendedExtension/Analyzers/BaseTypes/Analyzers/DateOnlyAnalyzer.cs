using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes = [typeof(UseBinaryOperatorSuggestion), typeof(UseExpressionResultSuggestion)])]
public sealed class DateOnlyAnalyzer : ElementProblemAnalyzer<IInvocationExpression>
{
    /// <remarks>
    /// <c>dateOnly.Equals(value)</c> → <c>dateOnly == value</c>
    /// </remarks>
    static void AnalyzeEquals_DateOnly(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ICSharpArgument valueArgument)
    {
        Debug.Assert(invokedExpression.QualifierExpression is { });

        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '==' operator.",
                    invocationExpression,
                    "==",
                    invokedExpression.QualifierExpression.GetText(),
                    valueArgument.Value.GetText()));
        }
    }

    /// <remarks>
    /// <c>dateOnly.Equals(null)</c> → <c>false</c>
    /// </remarks>
    static void AnalyzeEquals_Object(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument valueArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && valueArgument.Value.IsDefaultValue())
        {
            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always false.", invocationExpression, "false"));
        }
    }

    protected override void Run(IInvocationExpression element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element is { InvokedExpression: IReferenceExpression { Reference: var reference } invokedExpression }
            && reference.Resolve().DeclaredElement is IMethod
            {
                AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC, TypeParameters: [],
            } method
            && method.ContainingType.IsClrType(PredefinedType.DATE_ONLY_FQN))
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false }):
                    switch (method.ShortName)
                    {
                        case "Equals": // todo: nameof(DateOnly.Equals) when available
                            switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                            {
                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsDateOnly():
                                    AnalyzeEquals_DateOnly(consumer, element, invokedExpression, valueArgument);
                                    break;

                                case ([{ Type: var valueType }], [{ } valueArgument]) when valueType.IsObject():
                                    AnalyzeEquals_Object(consumer, element, valueArgument);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}