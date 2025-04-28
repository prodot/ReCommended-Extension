using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class SignedIntegerAnalyzer<N>(IClrTypeName clrTypeName) : IntegerAnalyzer<N>(clrTypeName) where N : struct
{
    /// <remarks>
    /// <c>T.IsNegative(n)</c> → <c>n &lt; 0</c>
    /// </remarks>
    static void AnalyzeIsNegative(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument objArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '<' operator to compare to 0.",
                    invocationExpression,
                    "<",
                    objArgument.Value.GetText(),
                    "0"));
        }
    }

    /// <remarks>
    /// <c>T.IsPositive(n)</c> → <c>n >= 0</c>
    /// </remarks>
    static void AnalyzeIsPositive(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument objArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && objArgument.Value is { })
        {
            consumer.AddHighlighting(
                new UseBinaryOperatorSuggestion(
                    "Use the '>=' operator to compare to 0.",
                    invocationExpression,
                    ">=",
                    objArgument.Value.GetText(),
                    "0"));
        }
    }

    /// <remarks>
    /// <c>T.MaxMagnitude(n, n)</c> → <c>n</c>
    /// </remarks>
    void AnalyzeMaxMagnitude(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument xArgument,
        ICSharpArgument yArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && TryGetConstant(xArgument.Value, out _) is { } x
            && TryGetConstant(yArgument.Value, out _) is { } y
            && AreEqual(x, y))
        {
            Debug.Assert(xArgument.Value is { });
            Debug.Assert(yArgument.Value is { });

            var replacementX = GetReplacementFromArgument(invocationExpression, xArgument.Value);
            var replacementY = GetReplacementFromArgument(invocationExpression, yArgument.Value);

            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {x}.",
                    invocationExpression,
                    replacementX,
                    replacementY != replacementX ? replacementY : null));
        }
    }

    /// <remarks>
    /// <c>T.MinMagnitude(n, n)</c> → <c>n</c>
    /// </remarks>
    void AnalyzeMinMagnitude(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument xArgument,
        ICSharpArgument yArgument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && TryGetConstant(xArgument.Value, out _) is { } x
            && TryGetConstant(yArgument.Value, out _) is { } y
            && AreEqual(x, y))
        {
            Debug.Assert(xArgument.Value is { });
            Debug.Assert(yArgument.Value is { });

            var replacementX = GetReplacementFromArgument(invocationExpression, xArgument.Value);
            var replacementY = GetReplacementFromArgument(invocationExpression, yArgument.Value);

            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    $"The expression is always {x}.",
                    invocationExpression,
                    replacementX,
                    replacementY != replacementX ? replacementY : null));
        }
    }

    private protected override void Analyze(
        IInvocationExpression element,
        IReferenceExpression invokedExpression,
        IMethod method,
        IHighlightingConsumer consumer)
    {
        base.Analyze(element, invokedExpression, method, consumer);

        if (method.ContainingType.IsClrType(ClrTypeName))
        {
            switch (invokedExpression, method)
            {
                case ({ QualifierExpression: { } }, { IsStatic: false }):
                    switch (method.ShortName) { }
                    break;

                case (_, { IsStatic: true }):
                    switch (method.ShortName)
                    {
                        case "IsNegative": // todo: nameof(INumberBase<T>.IsNegative) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsClrType(ClrTypeName):
                                    AnalyzeIsNegative(consumer, element, valueArgument);
                                    break;
                            }
                            break;

                        case "IsPositive": // todo: nameof(INumberBase<T>.IsPositive) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var valueType }], [var valueArgument]) when valueType.IsClrType(ClrTypeName):
                                    AnalyzeIsPositive(consumer, element, valueArgument);
                                    break;
                            }
                            break;

                        case "MaxMagnitude": // todo: nameof(INumberBase<T>.MaxMagnitude) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var xType }, { Type: var yType }], [var xArgument, var yArgument])
                                    when xType.IsClrType(ClrTypeName) && yType.IsClrType(ClrTypeName):

                                    AnalyzeMaxMagnitude(consumer, element, xArgument, yArgument);
                                    break;
                            }
                            break;

                        case "MinMagnitude": // todo: nameof(INumberBase<T>.MinMagnitude) when available
                            switch (method.Parameters, element.Arguments)
                            {
                                case ([{ Type: var xType }, { Type: var yType }], [var xArgument, var yArgument])
                                    when xType.IsClrType(ClrTypeName) && yType.IsClrType(ClrTypeName):

                                    AnalyzeMinMagnitude(consumer, element, xArgument, yArgument);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
    }
}