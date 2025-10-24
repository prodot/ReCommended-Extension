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
    HighlightingTypes = [typeof(UseExpressionResultSuggestion), typeof(UseUnaryOperatorSuggestion)])]
public sealed class DecimalAnalyzer() : FractionalNumberAnalyzer<decimal>(NumberInfos.NumberInfo.Decimal)
{
    /// <remarks>
    /// <c>decimal.Negate(d)</c> → <c>-d</c>
    /// </remarks>
    void AnalyzeNegate(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument dArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && dArgument.Value is { } value)
        {
            Debug.Assert(NumberInfo is { CastConstant: { }, Cast: { } });

            var operand = value.Type().IsDecimal()
                ? dArgument.Value.GetText()
                : NumberInfo.TryGetConstant(value, out var implicitlyConverted) is { } && implicitlyConverted
                    ? NumberInfo.CastConstant(value, implicitlyConverted)
                    : NumberInfo.Cast(value);

            consumer.AddHighlighting(new UseUnaryOperatorSuggestion("Use the '-' operator.", invocationExpression, "-", operand));
        }
    }

    private protected override void Analyze(
        IInvocationExpression element,
        IReferenceExpression invokedExpression,
        IMethod method,
        IHighlightingConsumer consumer)
    {
        base.Analyze(element, invokedExpression, method, consumer);

        if (method.ContainingType.IsClrType(NumberInfo.ClrTypeName) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case nameof(decimal.Negate):
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var dType }], [{ } dArgument]) when dType.IsDecimal(): AnalyzeNegate(consumer, element, dArgument); break;
                    }
                    break;
            }
        }
    }
}