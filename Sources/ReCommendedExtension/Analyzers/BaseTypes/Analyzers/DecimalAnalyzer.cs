using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

/// <remarks>
/// C# language version checks are only done when a quick fix would require it.
/// </remarks>
[ElementProblemAnalyzer(
    typeof(IInvocationExpression),
    HighlightingTypes =
    [
        typeof(UseExpressionResultSuggestion),
        typeof(UseBinaryOperatorSuggestion),
        typeof(RedundantArgumentHint),
        typeof(UseUnaryOperatorSuggestion),
        typeof(SuspiciousFormatSpecifierWarning),
        typeof(RedundantFormatPrecisionSpecifierHint),
    ])]
public sealed class DecimalAnalyzer() : FractionalNumberAnalyzer<decimal>(NumberInfo.Decimal)
{
    [Pure]
    static (string leftOperand, string rightOperand)? TryGetBinaryOperatorOperandsFromArguments(
        ICSharpArgument d1Argument,
        ICSharpArgument d2Argument)
    {
        if (d1Argument.Value is { } d1Value && d2Argument.Value is { } d2Value)
        {
            if (d1Value.Type().IsDecimal() || d2Value.Type().IsDecimal())
            {
                return (d1Value.GetText(), d2Value.GetText());
            }

            if (NumberInfo.Decimal.TryGetConstant(d1Value, out var d1ImplicitlyConverted) is { } && d1ImplicitlyConverted)
            {
                return (NumberInfo.Decimal.CastConstant(d1Value, d1ImplicitlyConverted), d2Value.GetText());
            }

            if (NumberInfo.Decimal.TryGetConstant(d2Value, out var d2ImplicitlyConverted) is { } && d2ImplicitlyConverted)
            {
                return (d1Value.GetText(), NumberInfo.Decimal.CastConstant(d2Value, d2ImplicitlyConverted));
            }

            return (NumberInfo.Decimal.Cast(d1Value), d2Value.GetText());
        }

        return null;
    }

    /// <remarks>
    /// <c>decimal.Add(d1, d2)</c> → <c>d1 + d2</c>
    /// </remarks>
    static void AnalyzeAdd(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument d1Argument,
        ICSharpArgument d2Argument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && TryGetBinaryOperatorOperandsFromArguments(d1Argument, d2Argument) is var (leftOperand, rightOperand))
        {
            consumer.AddHighlighting(new UseBinaryOperatorSuggestion("Use the '+' operator.", invocationExpression, "+", leftOperand, rightOperand));
        }
    }

    /// <remarks>
    /// <c>decimal.Divide(d1, d2)</c> → <c>d1 / d2</c>
    /// </remarks>
    static void AnalyzeDivide(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument d1Argument,
        ICSharpArgument d2Argument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && TryGetBinaryOperatorOperandsFromArguments(d1Argument, d2Argument) is var (leftOperand, rightOperand))
        {
            consumer.AddHighlighting(new UseBinaryOperatorSuggestion("Use the '/' operator.", invocationExpression, "/", leftOperand, rightOperand));
        }
    }

    /// <remarks>
    /// <c>decimal.Multiply(d1, d2)</c> → <c>d1 * d2</c>
    /// </remarks>
    static void AnalyzeMultiply(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument d1Argument,
        ICSharpArgument d2Argument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && TryGetBinaryOperatorOperandsFromArguments(d1Argument, d2Argument) is var (leftOperand, rightOperand))
        {
            consumer.AddHighlighting(new UseBinaryOperatorSuggestion("Use the '*' operator.", invocationExpression, "*", leftOperand, rightOperand));
        }
    }

    /// <remarks>
    /// <c>decimal.Negate(d)</c> → <c>-d</c>
    /// </remarks>
    static void AnalyzeNegate(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument dArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && dArgument.Value is { } value)
        {
            var operand = value.Type().IsDecimal()
                ? dArgument.Value.GetText()
                : NumberInfo.Decimal.TryGetConstant(value, out var implicitlyConverted) is { } && implicitlyConverted
                    ? NumberInfo.Decimal.CastConstant(value, implicitlyConverted)
                    : NumberInfo.Decimal.Cast(value);

            consumer.AddHighlighting(new UseUnaryOperatorSuggestion("Use the '-' operator.", invocationExpression, "-", operand));
        }
    }

    /// <remarks>
    /// <c>decimal.Remainder(d1, d2)</c> → <c>d1 % d2</c>
    /// </remarks>
    static void AnalyzeRemainder(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument d1Argument,
        ICSharpArgument d2Argument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && TryGetBinaryOperatorOperandsFromArguments(d1Argument, d2Argument) is var (leftOperand, rightOperand))
        {
            consumer.AddHighlighting(new UseBinaryOperatorSuggestion("Use the '%' operator.", invocationExpression, "%", leftOperand, rightOperand));
        }
    }

    /// <remarks>
    /// <c>decimal.Subtract(d1, d2)</c> → <c>d1 - d2</c>
    /// </remarks>
    static void AnalyzeSubtract(
        IHighlightingConsumer consumer,
        IInvocationExpression invocationExpression,
        ICSharpArgument d1Argument,
        ICSharpArgument d2Argument)
    {
        if (!invocationExpression.IsUsedAsStatement()
            && TryGetBinaryOperatorOperandsFromArguments(d1Argument, d2Argument) is var (leftOperand, rightOperand))
        {
            consumer.AddHighlighting(new UseBinaryOperatorSuggestion("Use the '-' operator.", invocationExpression, "-", leftOperand, rightOperand));
        }
    }

    private protected override void Analyze(
        IInvocationExpression element,
        IReferenceExpression invokedExpression,
        IMethod method,
        IHighlightingConsumer consumer)
    {
        base.Analyze(element, invokedExpression, method, consumer);

        if (method.ContainingType.IsClrType(NumberInfo.Decimal.ClrTypeName) && method.IsStatic)
        {
            switch (method.ShortName)
            {
                case nameof(decimal.Add):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var d1Type }, { Type: var d2Type }], [var d1Argument, var d2Argument])
                            when d1Type.IsDecimal() && d2Type.IsDecimal():

                            AnalyzeAdd(consumer, element, d1Argument, d2Argument);
                            break;
                    }
                    break;

                case nameof(decimal.Divide):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var d1Type }, { Type: var d2Type }], [var d1Argument, var d2Argument])
                            when d1Type.IsDecimal() && d2Type.IsDecimal():

                            AnalyzeDivide(consumer, element, d1Argument, d2Argument);
                            break;
                    }
                    break;

                case nameof(decimal.Multiply):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var d1Type }, { Type: var d2Type }], [var d1Argument, var d2Argument])
                            when d1Type.IsDecimal() && d2Type.IsDecimal():

                            AnalyzeMultiply(consumer, element, d1Argument, d2Argument);
                            break;
                    }
                    break;

                case nameof(decimal.Negate):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var dType }], [var dArgument]) when dType.IsDecimal(): AnalyzeNegate(consumer, element, dArgument); break;
                    }
                    break;

                case nameof(decimal.Remainder):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var d1Type }, { Type: var d2Type }], [var d1Argument, var d2Argument])
                            when d1Type.IsDecimal() && d2Type.IsDecimal():

                            AnalyzeRemainder(consumer, element, d1Argument, d2Argument);
                            break;
                    }
                    break;

                case nameof(decimal.Subtract):
                    switch (method.Parameters, element.Arguments)
                    {
                        case ([{ Type: var d1Type }, { Type: var d2Type }], [var d1Argument, var d2Argument])
                            when d1Type.IsDecimal() && d2Type.IsDecimal():

                            AnalyzeSubtract(consumer, element, d1Argument, d2Argument);
                            break;
                    }
                    break;
            }
        }
    }
}