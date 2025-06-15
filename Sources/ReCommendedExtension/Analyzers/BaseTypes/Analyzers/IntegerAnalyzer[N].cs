using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class IntegerAnalyzer<N>(NumberInfo<N> numberInfo) : NumberAnalyzer<N>(numberInfo) where N : struct
{
    /// <remarks>
    /// <c>T.DivRem(0, right)</c> → <c>(0, 0)</c>
    /// </remarks>
    void AnalyzeDivRem(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument leftArgument,
        ICSharpArgument rightArgument)
    {
        Debug.Assert(NumberInfo.IsZero is { });

        if (!invocationExpression.IsUsedAsStatement()
            && NumberInfo.TryGetConstant(rightArgument.Value, out _) is { } right
            && NumberInfo.TryGetConstant(leftArgument.Value, out _) is { } left
            && NumberInfo.IsZero(left)
            && !NumberInfo.IsZero(right))
        {
            Debug.Assert(NumberInfo.CastZero is { });

            var replacement =
                invocationExpression.TryGetTargetType(false).IsValueTuple(out var t1TypeArgument, out var t2TypeArgument)
                && t1TypeArgument.IsClrType(NumberInfo.ClrTypeName)
                && t2TypeArgument.IsClrType(NumberInfo.ClrTypeName)
                    ? "(0, 0)"
                    : $"(Quotient: {
                        NumberInfo.CastZero(invocationExpression.GetCSharpLanguageLevel())
                    }, Remainder: {
                        NumberInfo.CastZero(invocationExpression.GetCSharpLanguageLevel())
                    })";

            consumer.AddHighlighting(new UseExpressionResultSuggestion("The expression is always (0, 0).", invocationExpression, replacement));
        }
    }

    /// <remarks>
    /// <c>T.RotateLeft(n, 0)</c> → <c>n</c>
    /// </remarks>
    void AnalyzeRotateLeft(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument rotateAmountArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && rotateAmountArgument.Value.TryGetInt32Constant() == 0 && valueArgument.Value is { } value)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    "The expression is always the same as the first argument.",
                    invocationExpression,
                    NumberInfo.GetReplacementFromArgument(invocationExpression, value)));
        }
    }

    /// <remarks>
    /// <c>T.RotateRight(n, 0)</c> → <c>n</c>
    /// </remarks>
    void AnalyzeRotateRight(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument valueArgument,
        ICSharpArgument rotateAmountArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && rotateAmountArgument.Value.TryGetInt32Constant() == 0 && valueArgument.Value is { } value)
        {
            consumer.AddHighlighting(
                new UseExpressionResultSuggestion(
                    "The expression is always the same as the first argument.",
                    invocationExpression,
                    NumberInfo.GetReplacementFromArgument(invocationExpression, value)));
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
                case "DivRem": // todo: nameof(IBinaryInteger<T>.DivRem) when available
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var leftType }, { Type: var rightType }], [var leftArgument, var rightArgument])
                            when leftType.IsClrType(NumberInfo.ClrTypeName) && rightType.IsClrType(NumberInfo.ClrTypeName):

                            AnalyzeDivRem(consumer, element, leftArgument, rightArgument);
                            break;
                    }
                    break;

                case "RotateLeft": // todo: nameof(IBinaryInteger<T>.RotateLeft) when available
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }, { Type: var rotateAmountType }], [var valueArgument, var rotateAmountArgument])
                            when valueType.IsClrType(NumberInfo.ClrTypeName) && rotateAmountType.IsInt():

                            AnalyzeRotateLeft(consumer, element, valueArgument, rotateAmountArgument);
                            break;
                    }
                    break;

                case "RotateRight": // todo: nameof(IBinaryInteger<T>.RotateRight) when available
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }, { Type: var rotateAmountType }], [var valueArgument, var rotateAmountArgument])
                            when valueType.IsClrType(NumberInfo.ClrTypeName) && rotateAmountType.IsInt():

                            AnalyzeRotateRight(consumer, element, valueArgument, rotateAmountArgument);
                            break;
                    }
                    break;
            }
        }

        if (method.ContainingType.IsClrType(ClrTypeNames.Math) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case nameof(Math.DivRem):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var leftType }, { Type: var rightType }], [var leftArgument, var rightArgument])
                            when leftType.IsClrType(NumberInfo.ClrTypeName) && rightType.IsClrType(NumberInfo.ClrTypeName):

                            AnalyzeDivRem(consumer, element, leftArgument, rightArgument);
                            break;
                    }
                    break;
            }
        }
    }
}