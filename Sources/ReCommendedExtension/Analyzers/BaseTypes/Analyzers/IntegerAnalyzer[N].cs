using System.Globalization;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

public abstract class IntegerAnalyzer<N>(IClrTypeName clrTypeName) : NumberAnalyzer<N>(clrTypeName) where N : struct
{
    /// <remarks>
    /// <c>T.DivRem(0, right)</c> → <c>(0, 0)</c><para/>
    /// <c>T.DivRem(left, 1)</c> → <c>(left, 0)</c>
    /// </remarks>
    void AnalyzeDivRem(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument leftArgument,
        ICSharpArgument rightArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && TryGetConstant(rightArgument.Value, out _) is { } right)
        {
            if (TryGetConstant(leftArgument.Value, out _) is { } left && IsZero(left) && !IsZero(right))
            {
                var replacement =
                    invocationExpression.TryGetTargetType().IsValueTuple(out var t1TypeArgument, out var t2TypeArgument)
                    && t1TypeArgument.IsClrType(ClrTypeName)
                    && t2TypeArgument.IsClrType(ClrTypeName)
                        ? "(0, 0)"
                        : $"(Quotient: {CastZero(invocationExpression.GetCSharpLanguageLevel())}, Remainder: {CastZero(invocationExpression.GetCSharpLanguageLevel())})";

                consumer.AddHighlighting(
                    new UseExpressionResultSuggestion("The expression is always (0, 0).", invocationExpression, replacement));
            }
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
                    GetReplacementFromArgument(invocationExpression, value)));
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
                    GetReplacementFromArgument(invocationExpression, value)));
        }
    }

    private protected sealed override NumberStyles GetDefaultNumberStyles() => NumberStyles.Integer;

    private protected sealed override bool CanUseEqualityOperator() => true;

    [Pure]
    private protected abstract string CastZero(CSharpLanguageLevel languageLevel);

    [Pure]
    private protected abstract bool IsZero(N value);

    private protected override void Analyze(
        IInvocationExpression element,
        IReferenceExpression invokedExpression,
        IMethod method,
        IHighlightingConsumer consumer)
    {
        base.Analyze(element, invokedExpression, method, consumer);

        if (method.ContainingType.IsClrType(ClrTypeName) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case "DivRem": // todo: nameof(IBinaryInteger<T>.DivRem) when available
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var leftType }, { Type: var rightType }], [var leftArgument, var rightArgument])
                            when leftType.IsClrType(ClrTypeName) && rightType.IsClrType(ClrTypeName):

                            AnalyzeDivRem(consumer, element, leftArgument, rightArgument);
                            break;
                    }
                    break;

                case "RotateLeft": // todo: nameof(IBinaryInteger<T>.RotateLeft) when available
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }, { Type: var rotateAmountType }], [var valueArgument, var rotateAmountArgument])
                            when valueType.IsClrType(ClrTypeName) && rotateAmountType.IsInt():

                            AnalyzeRotateLeft(consumer, element, valueArgument, rotateAmountArgument);
                            break;
                    }
                    break;

                case "RotateRight": // todo: nameof(IBinaryInteger<T>.RotateRight) when available
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var valueType }, { Type: var rotateAmountType }], [var valueArgument, var rotateAmountArgument])
                            when valueType.IsClrType(ClrTypeName) && rotateAmountType.IsInt():

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
                            when leftType.IsClrType(ClrTypeName) && rightType.IsClrType(ClrTypeName):

                            AnalyzeDivRem(consumer, element, leftArgument, rightArgument);
                            break;
                    }
                    break;
            }
        }
    }
}