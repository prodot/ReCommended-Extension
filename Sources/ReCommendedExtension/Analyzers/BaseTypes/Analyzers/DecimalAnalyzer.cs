using System.Globalization;
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
    HighlightingTypes =
    [
        typeof(UseExpressionResultSuggestion),
        typeof(UseBinaryOperatorSuggestion),
        typeof(RedundantArgumentHint),
        typeof(UseUnaryOperatorSuggestion),
    ])]
public sealed class DecimalAnalyzer() : NumberAnalyzer<decimal>(PredefinedType.DECIMAL_FQN)
{
    [Pure]
    (string leftOperand, string rightOperand)? TryGetBinaryOperatorOperandsFromArguments(ICSharpArgument d1Argument, ICSharpArgument d2Argument)
    {
        if (d1Argument.Value is { } d1Value && d2Argument.Value is { } d2Value)
        {
            if (d1Value.Type().IsDecimal() || d2Value.Type().IsDecimal())
            {
                return (d1Value.GetText(), d2Value.GetText());
            }

            if (TryGetConstant(d1Value, out var d1ImplicitlyConverted) is { } && d1ImplicitlyConverted)
            {
                return (CastConstant(d1Value, d1ImplicitlyConverted), d2Value.GetText());
            }

            if (TryGetConstant(d2Value, out var d2ImplicitlyConverted) is { } && d2ImplicitlyConverted)
            {
                return (d1Value.GetText(), CastConstant(d2Value, d2ImplicitlyConverted));
            }

            return (Cast(d1Value), d2Value.GetText());
        }

        return null;
    }

    /// <remarks>
    /// <c>decimal.Add(d1, d2)</c> → <c>d1 + d2</c>
    /// </remarks>
    void AnalyzeAdd(
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
    void AnalyzeDivide(
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
    void AnalyzeMultiply(
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
    void AnalyzeNegate(IHighlightingConsumer consumer, IInvocationExpression invocationExpression, ICSharpArgument dArgument)
    {
        if (!invocationExpression.IsUsedAsStatement() && dArgument.Value is { } value)
        {
            var operand = value.Type().IsDecimal()
                ? dArgument.Value.GetText()
                : TryGetConstant(value, out var implicitlyConverted) is { } && implicitlyConverted
                    ? CastConstant(value, implicitlyConverted)
                    : Cast(value);

            consumer.AddHighlighting(new UseUnaryOperatorSuggestion("Use the '-' operator.", invocationExpression, "-", operand));
        }
    }

    /// <remarks>
    /// <c>decimal.Remainder(d1, d2)</c> → <c>d1 % d2</c>
    /// </remarks>
    void AnalyzeRemainder(
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
    void AnalyzeSubtract(
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

    private protected override TypeCode? TryGetTypeCode() => TypeCode.Decimal;

    private protected override NumberStyles GetDefaultNumberStyles() => NumberStyles.Number;

    private protected override bool CanUseEqualityOperator() => true;

    private protected override decimal? TryGetConstant(ICSharpExpression? expression, out bool implicitlyConverted)
    {
        if (expression is IConstantValueOwner constantValueOwner)
        {
            switch (constantValueOwner.ConstantValue)
            {
                case { Kind: ConstantValueKind.Decimal, DecimalValue: var value }:
                    implicitlyConverted = false;
                    return value;

                case { Kind: ConstantValueKind.Ulong, UlongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Long, LongValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Int, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Uint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Byte, ByteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Sbyte, SbyteValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Short, ShortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Ushort, UshortValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nint, IntValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Nuint, UintValue: var value }:
                    implicitlyConverted = true;
                    return value;

                case { Kind: ConstantValueKind.Char, CharValue: var value }:
                    implicitlyConverted = true;
                    return value;
            }
        }

        implicitlyConverted = false;
        return null;
    }

    private protected override string CastConstant(ICSharpExpression constant, bool implicitlyConverted)
    {
        if (implicitlyConverted)
        {
            if (constant is ICSharpLiteralExpression)
            {
                if (constant.Type().IsChar())
                {
                    return constant.Cast("decimal").GetText();
                }

                var result = constant.GetText();

                var magnitude = result is ['-' or '+', .. var m] ? m : result;
                if (magnitude is ['0', 'x' or 'X' or 'b' or 'B', ..])
                {
                    return constant.Cast("decimal").GetText();
                }

                return result switch
                {
                    [.. var rest, 'u' or 'U', 'l' or 'L'] => $"{rest}m",
                    [.. var rest, 'l' or 'L', 'u' or 'U'] => $"{rest}m",
                    [.. var rest, 'u' or 'U'] => $"{rest}m",
                    [.. var rest, 'l' or 'L'] => $"{rest}m",

                    _ => $"{result}m",
                };
            }

            return constant.Cast("decimal").GetText();
        }

        return constant.GetText();
    }

    private protected override string Cast(ICSharpExpression expression) => expression.Cast("decimal").GetText();

    private protected override bool AreEqual(decimal x, decimal y) => x == y;

    private protected override bool AreMinMaxValues(decimal min, decimal max) => (min, max) == (decimal.MinValue, decimal.MaxValue);

    private protected override void Analyze(IInvocationExpression element, IReferenceExpression invokedExpression, IMethod method, IHighlightingConsumer consumer)
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
                                case ([{ Type: var dType }], [var dArgument]) when dType.IsDecimal():
                                    AnalyzeNegate(consumer, element, dArgument);
                                    break;
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
                    break;
            }
        }
    }
}