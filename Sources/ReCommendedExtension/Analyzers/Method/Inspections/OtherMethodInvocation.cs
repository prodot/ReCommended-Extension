using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;
using ReCommendedExtension.Analyzers.Method.Rules;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Method.Inspections;

internal sealed record OtherMethodInvocation : Inspection
{
    record struct BinaryOperatorExpression(
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

    abstract record BinaryOperatorExpressionOperand;

    sealed record InvocationExpression : BinaryOperatorExpressionOperand
    {
        public static InvocationExpression Default { get; } = new();
    }

    sealed record Number : BinaryOperatorExpressionOperand
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

    enum Operator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }

    [Pure]
    static InvocationReplacement? TryGetReplacementForBinaryExpression(
        IInvocationExpression invocationExpression,
        Func<BinaryOperatorExpression, MethodInvocationContext?> tryGetContext,
        Func<IReadOnlyList<string>?> tryCreateArguments,
        bool isNegated)
        => BinaryOperatorExpression.TryFrom(invocationExpression) is { } binaryExpression
            && tryGetContext(binaryExpression) is { } context
            && tryCreateArguments() is { } arguments
                ? new InvocationReplacement
                {
                    OriginalExpression = binaryExpression.Expression, Context = context, Arguments = arguments, IsNegated = isNegated,
                }
                : null;

    public static OtherMethodInvocation ComparingResultAsContains { get; } = new()
    {
        TryGetReplacement = (invocationExpression, args) => TryGetReplacementForBinaryExpression(
            invocationExpression,
            binaryExpression => binaryExpression switch
            {
                (InvocationExpression, Operator.NotEqual, Number { Value: -1 }) => MethodInvocationContext.BinaryLeftOperand,
                (InvocationExpression, Operator.GreaterThan, Number { Value: -1 }) => MethodInvocationContext.BinaryLeftOperand,
                (InvocationExpression, Operator.GreaterThanOrEqual, Number { Value: 0 }) => MethodInvocationContext.BinaryLeftOperand,

                (Number { Value: -1 }, Operator.NotEqual, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,
                (Number { Value: -1 }, Operator.LessThan, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,
                (Number { Value: 0 }, Operator.LessThanOrEqual, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,

                _ => null,
            },
            () => args.All(a => a is { Value: { } }) ? [..from arg in args select arg.Value!.GetText()] : null,
            false),
        EnsureQualifierNotNull = true,
        Message = methodName => $"Use the '{methodName}' method.",
    };

    public static OtherMethodInvocation ComparingResultAsNotContains { get; } = new()
    {
        TryGetReplacement = (invocationExpression, args) => TryGetReplacementForBinaryExpression(
            invocationExpression,
            binaryExpression => binaryExpression switch
            {
                (InvocationExpression, Operator.Equal, Number { Value: -1 }) => MethodInvocationContext.BinaryLeftOperand,
                (InvocationExpression, Operator.LessThan, Number { Value: 0 }) => MethodInvocationContext.BinaryLeftOperand,

                (Number { Value: -1 }, Operator.Equal, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,
                (Number { Value: 0 }, Operator.GreaterThan, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,

                _ => null,
            },
            () => args.All(a => a is { Value: { } }) ? [..from arg in args select arg.Value!.GetText()] : null,
            true),
        EnsureQualifierNotNull = true,
        Message = methodName => $"Use the negated '{methodName}' method.",
    };

    public static OtherMethodInvocation ComparingResultAsContainsWithCurrentCulture { get; } = new()
    {
        TryGetReplacement = (invocationExpression, args) => TryGetReplacementForBinaryExpression(
            invocationExpression,
            binaryExpression => binaryExpression switch
            {
                (InvocationExpression, Operator.NotEqual, Number { Value: -1 }) => MethodInvocationContext.BinaryLeftOperand,
                (InvocationExpression, Operator.GreaterThan, Number { Value: -1 }) => MethodInvocationContext.BinaryLeftOperand,
                (InvocationExpression, Operator.GreaterThanOrEqual, Number { Value: 0 }) => MethodInvocationContext.BinaryLeftOperand,

                (Number { Value: -1 }, Operator.NotEqual, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,
                (Number { Value: -1 }, Operator.LessThan, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,
                (Number { Value: 0 }, Operator.LessThanOrEqual, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,

                _ => null,
            },
            () => args[0] is { Value: { } argValue }
                ? [argValue.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"]
                : null,
            false),
        EnsureQualifierNotNull = true,
        Message = methodName => $"Use the '{methodName}' method.",
    };

    public static OtherMethodInvocation ComparingResultAsNotContainsWithCurrentCulture { get; } = new()
    {
        TryGetReplacement = (invocationExpression, args) => TryGetReplacementForBinaryExpression(
            invocationExpression,
            binaryExpression => binaryExpression switch
            {
                (InvocationExpression, Operator.Equal, Number { Value: -1 }) => MethodInvocationContext.BinaryLeftOperand,
                (InvocationExpression, Operator.LessThan, Number { Value: 0 }) => MethodInvocationContext.BinaryLeftOperand,

                (Number { Value: -1 }, Operator.Equal, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,
                (Number { Value: 0 }, Operator.GreaterThan, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,

                _ => null,
            },
            () => args[0] is { Value: { } argValue }
                ? [argValue.GetText(), $"{nameof(StringComparison)}.{nameof(StringComparison.CurrentCulture)}"]
                : null,
            true),
        EnsureQualifierNotNull = true,
        Message = methodName => $"Use the negated '{methodName}' method.",
    };

    public static OtherMethodInvocation ComparingResultAsStartsWith { get; } = new()
    {
        TryGetReplacement = (invocationExpression, args) => TryGetReplacementForBinaryExpression(
            invocationExpression,
            binaryExpression => binaryExpression switch
            {
                (InvocationExpression, Operator.Equal, Number { Value: 0 }) => MethodInvocationContext.BinaryLeftOperand,
                (Number { Value: 0 }, Operator.Equal, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,

                _ => null,
            },
            () => args.All(a => a is { Value: { } }) ? [..from arg in args select arg.Value!.GetText()] : null,
            false),
        EnsureQualifierNotNull = true,
        Message = methodName => $"Use the '{methodName}' method.",
    };

    public static OtherMethodInvocation ComparingResultAsNotStartsWith { get; } = new()
    {
        TryGetReplacement = (invocationExpression, args) => TryGetReplacementForBinaryExpression(
            invocationExpression,
            binaryExpression => binaryExpression switch
            {
                (InvocationExpression, Operator.NotEqual, Number { Value: 0 }) => MethodInvocationContext.BinaryLeftOperand,
                (Number { Value: 0 }, Operator.NotEqual, InvocationExpression) => MethodInvocationContext.BinaryRightOperand,

                _ => null,
            },
            () => args.All(a => a is { Value: { } }) ? [..from arg in args select arg.Value!.GetText()] : null,
            true),
        EnsureQualifierNotNull = true,
        Message = methodName => $"Use the negated '{methodName}' method.",
    };

    public static OtherMethodInvocation SingleElementCollectionWithFurtherArguments { get; } = new()
    {
        TryGetReplacement =
            (invocationExpression, args)
                => CollectionCreation.TryFrom(args[0]?.Value) is { Count: 1 } collectionCreation && args.Skip(1).All(a => a is { Value: { } })
                    ? new InvocationReplacement
                    {
                        OriginalExpression = invocationExpression,
                        Context = MethodInvocationContext.Standalone,
                        Arguments = [collectionCreation.SingleElement.GetText(), ..from arg in args.Skip(1) select arg.Value!.GetText()],
                        IsNegated = false,
                    }
                    : null,
        Message = methodName => $"Use the '{methodName}' method.",
    };

    [Pure]
    static InvocationReplacement? TryGetReplacementForSingleElementParamsCollectionInArg1WithoutArg0(
        IInvocationExpression invocationExpression,
        TreeNodeCollection<ICSharpArgument?> arguments,
        Func<IType, bool> isCollectionTypeToExclude)
    {
        if (arguments is [_, var arg])
        {
            if (CollectionCreation.TryFrom(arg?.Value) is { } collectionCreation)
            {
                // passed as an explicit collection creation

                if (collectionCreation.Count == 1)
                {
                    return new InvocationReplacement
                    {
                        OriginalExpression = invocationExpression,
                        Context = MethodInvocationContext.Standalone,
                        Arguments = [collectionCreation.SingleElement.GetText()],
                        IsNegated = false,
                    };
                }
            }
            else
            {
                if (arg?.Value?.GetExpressionType().ToIType() is { } argType && !isCollectionTypeToExclude(argType))
                {
                    // passed not as an explicit collection creation (collection created by the "params" modifier)

                    return new InvocationReplacement
                    {
                        OriginalExpression = invocationExpression,
                        Context = MethodInvocationContext.Standalone,
                        Arguments = [arg.Value.GetText()],
                        IsNegated = false,
                    };
                }
            }
        }

        return null;
    }

    public static OtherMethodInvocation SingleElementIEnumerableOfTInArg1WithoutArg0 { get; } = new()
    {
        TryGetReplacement =
            (invocationExpression, args) => TryGetReplacementForSingleElementParamsCollectionInArg1WithoutArg0(
                invocationExpression,
                args,
                _ => true), // not a "params" modifier
        Message = methodName => $"Use the '{methodName}' method.",
    };

    public static OtherMethodInvocation SingleElementParamsStringArrayInArg1WithoutArg0 { get; } = new()
    {
        TryGetReplacement =
            (invocationExpression, args) => TryGetReplacementForSingleElementParamsCollectionInArg1WithoutArg0(
                invocationExpression,
                args,
                type => type.IsGenericArrayOfString()),
        Message = methodName => $"Use the '{methodName}' method.",
    };

    public static OtherMethodInvocation SingleElementParamsObjectArrayInArg1WithoutArg0 { get; } = new()
    {
        TryGetReplacement =
            (invocationExpression, args) => TryGetReplacementForSingleElementParamsCollectionInArg1WithoutArg0(
                invocationExpression,
                args,
                type => type.IsGenericArrayOfObject()),
        Message = methodName => $"Use the '{methodName}' method.",
    };

    public static OtherMethodInvocation SingleElementParamsReadOnlySpanOfStringInArg1WithoutArg0 { get; } = new()
    {
        TryGetReplacement =
            (invocationExpression, args) => TryGetReplacementForSingleElementParamsCollectionInArg1WithoutArg0(
                invocationExpression,
                args,
                type => type.IsReadOnlySpanOfString()),
        Message = methodName => $"Use the '{methodName}' method.",
    };

    public static OtherMethodInvocation SingleElementParamsReadOnlySpanOfObjectInArg1WithoutArg0 { get; } = new()
    {
        TryGetReplacement =
            (invocationExpression, args) => TryGetReplacementForSingleElementParamsCollectionInArg1WithoutArg0(
                invocationExpression,
                args,
                type => type.IsReadOnlySpanOfObject()),
        Message = methodName => $"Use the '{methodName}' method.",
    };

    public bool EnsureQualifierNotNull { get; private init; }

    public required Func<IInvocationExpression, TreeNodeCollection<ICSharpArgument?>, InvocationReplacement?> TryGetReplacement { get; init; }

    public ReplacementMethod? ReplacementMethod { get; init; }
}