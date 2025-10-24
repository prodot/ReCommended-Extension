using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class SignedIntegerAnalyzer<N>(NumberInfo<N> numberInfo) : IntegerAnalyzer<N>(numberInfo) where N : struct
{
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
            && NumberInfo.TryGetConstant(xArgument.Value, out _) is { } x
            && NumberInfo.TryGetConstant(yArgument.Value, out _) is { } y
            && NumberInfo.AreEqual(x, y))
        {
            Debug.Assert(xArgument.Value is { });
            Debug.Assert(yArgument.Value is { });

            var replacementX = NumberInfo.GetReplacementFromArgument(invocationExpression, xArgument.Value);
            var replacementY = NumberInfo.GetReplacementFromArgument(invocationExpression, yArgument.Value);

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
            && NumberInfo.TryGetConstant(xArgument.Value, out _) is { } x
            && NumberInfo.TryGetConstant(yArgument.Value, out _) is { } y
            && NumberInfo.AreEqual(x, y))
        {
            Debug.Assert(xArgument.Value is { });
            Debug.Assert(yArgument.Value is { });

            var replacementX = NumberInfo.GetReplacementFromArgument(invocationExpression, xArgument.Value);
            var replacementY = NumberInfo.GetReplacementFromArgument(invocationExpression, yArgument.Value);

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

        if (method.ContainingType.IsClrType(NumberInfo.ClrTypeName) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case "MaxMagnitude": // todo: nameof(INumberBase<T>.MaxMagnitude) when available
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var xType }, { Type: var yType }], [{ } xArgument, { } yArgument])
                            when xType.IsClrType(NumberInfo.ClrTypeName) && yType.IsClrType(NumberInfo.ClrTypeName):

                            AnalyzeMaxMagnitude(consumer, element, xArgument, yArgument);
                            break;
                    }
                    break;

                case "MinMagnitude": // todo: nameof(INumberBase<T>.MinMagnitude) when available
                    switch (method.Parameters, element.TryGetArgumentsInDeclarationOrder())
                    {
                        case ([{ Type: var xType }, { Type: var yType }], [{ } xArgument, { } yArgument])
                            when xType.IsClrType(NumberInfo.ClrTypeName) && yType.IsClrType(NumberInfo.ClrTypeName):

                            AnalyzeMinMagnitude(consumer, element, xArgument, yArgument);
                            break;
                    }
                    break;
            }
        }
    }
}