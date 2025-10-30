using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal abstract record Inspection
{
    protected record struct BinaryOperatorExpression(
        BinaryOperatorExpressionOperand LeftOperand,
        Operator Operator,
        BinaryOperatorExpressionOperand RightOperand)
    {
        [Pure]
        static Operator? TryGetOperator(IEqualityExpression equalityExpression)
            => equalityExpression.EqualityType switch
            {
                EqualityExpressionType.EQEQ => Operator.Equal,
                EqualityExpressionType.NE => Operator.NotEqual,

                _ => null,
            };

        [Pure]
        static Operator? TryGetOperator(IRelationalExpression relationalExpression)
            => relationalExpression.OperatorSign.GetTokenType() switch
            {
                var t when t == CSharpTokenType.GT => Operator.GreaterThan,
                var t when t == CSharpTokenType.LT => Operator.LessThan,
                var t when t == CSharpTokenType.GE => Operator.GreaterThanOrEqual,
                var t when t == CSharpTokenType.LE => Operator.LessThanOrEqual,

                _ => null,
            };

        [Pure]
        public static BinaryOperatorExpression? TryFrom(IInvocationExpression invocationExpression)
        {
            if (invocationExpression.Parent is IBinaryExpression binaryExpression)
            {
                var binaryOperator = binaryExpression switch
                {
                    IEqualityExpression equalityExpression => TryGetOperator(equalityExpression),
                    IRelationalExpression relationalExpression => TryGetOperator(relationalExpression),

                    _ => null,
                };

                if (binaryOperator is { } op)
                {
                    if (binaryExpression.LeftOperand == invocationExpression
                        && binaryExpression.RightOperand.TryGetInt32Constant() is { } rightOperandValue)
                    {
                        return new BinaryOperatorExpression(InvocationExpression.Default, op, Number.From(rightOperandValue))
                        {
                            Expression = binaryExpression,
                        };
                    }

                    if (binaryExpression.LeftOperand.TryGetInt32Constant() is { } leftOperandValue
                        && binaryExpression.RightOperand == invocationExpression)
                    {
                        return new BinaryOperatorExpression(Number.From(leftOperandValue), op, InvocationExpression.Default)
                        {
                            Expression = binaryExpression,
                        };
                    }
                }
            }

            return null;
        }

        public required IBinaryExpression Expression { get; init; }
    }

    protected abstract record BinaryOperatorExpressionOperand;

    protected sealed record InvocationExpression : BinaryOperatorExpressionOperand
    {
        public static InvocationExpression Default { get; } = new();
    }

    protected sealed record Number : BinaryOperatorExpressionOperand
    {
        static readonly Number zero = new() { Value = 0 };
        static readonly Number minusOne = new() { Value = -1 };

        [Pure]
        public static Number From(int value)
            => value switch
            {
                0 => zero,
                -1 => minusOne,
                _ => new Number { Value = value },
            };

        public required int Value { get; init; }
    }

    protected enum Operator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }

    public required Func<string, string> Message { get; init; }
}