using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
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

    [Pure]
    static IEnumerable<IProperty> GetAllProperties(ITypeElement typeElement)
        => typeElement.Properties.Concat(
            from baseTypeElement in typeElement.GetAllSuperTypeElements() from property in baseTypeElement.Properties select property);

    [Pure]
    static IEnumerable<IProperty> GetIndexers(IEnumerable<IProperty> properties)
        => from property in properties where property.Parameters is [{ Type: var parameterType }] && parameterType.IsInt() select property;

    [Pure]
    protected static bool IsIndexableCollectionOrString(IType type, ITreeNode context)
    {
        if (type.IsGenericIList() || type.IsGenericIReadOnlyList() || type.IsGenericList() || type.IsGenericArray() || type.IsString())
        {
            return true;
        }

        if (type.GetTypeElement() is { } typeElement)
        {
            var psiModule = context.GetPsiModule();

            return typeElement.IsDescendantOf(PredefinedType.GENERIC_ILIST_FQN.TryGetTypeElement(psiModule))
                || typeElement.IsDescendantOf(ClrTypeNames.IReadOnlyList.TryGetTypeElement(psiModule));
        }

        return false;
    }

    [Pure]
    protected static bool IsIndexableCollectionOrString(IType type, ICSharpExpression expression, out bool hasAccessibleIndexer)
    {
        if (type.IsGenericIList() || type.IsGenericIReadOnlyList() || type.IsGenericList() || type.IsGenericArray() || type.IsString())
        {
            hasAccessibleIndexer = true;
            return true;
        }

        if (type.GetTypeElement() is { } typeElement)
        {
            var psiModule = expression.GetPsiModule();

            var implementedListTypeElement = PredefinedType.GENERIC_ILIST_FQN.TryGetTypeElement(psiModule) is { } t1 && typeElement.IsDescendantOf(t1)
                ? t1
                : null;

            var implementedReadOnlyListTypeElement =
                ClrTypeNames.IReadOnlyList.TryGetTypeElement(psiModule) is { } t2 && typeElement.IsDescendantOf(t2) ? t2 : null;

            if (implementedListTypeElement is { } || implementedReadOnlyListTypeElement is { })
            {
                if (typeElement is ITypeParameter typeParameter)
                {
                    var hasAccessibleIndexerIfIndexableCollection = false;
                    if (typeParameter.TypeConstraints.Any(t => IsIndexableCollectionOrString(
                        t,
                        expression,
                        out hasAccessibleIndexerIfIndexableCollection)))
                    {
                        hasAccessibleIndexer = hasAccessibleIndexerIfIndexableCollection;
                        return true;
                    }
                }

                var listIndexer = implementedListTypeElement is { } ? GetIndexers(implementedListTypeElement.Properties).FirstOrDefault() : null;
                var readOnlyListIndexer = implementedReadOnlyListTypeElement is { }
                    ? GetIndexers(implementedReadOnlyListTypeElement.Properties).FirstOrDefault()
                    : null;

                hasAccessibleIndexer = GetIndexers(GetAllProperties(typeElement))
                    .Any(indexer
                        => (listIndexer is { } && indexer.OverridesOrImplements(listIndexer)
                            || readOnlyListIndexer is { } && indexer.OverridesOrImplements(readOnlyListIndexer))
                        && AccessUtil.IsSymbolAccessible(indexer, new ElementAccessContext(expression)));
                return true;
            }
        }

        hasAccessibleIndexer = false;
        return false;
    }

    [Pure]
    protected static bool IsDistinctCollection(IType type, ITreeNode context)
    {
        if (type.IsClrType(PredefinedType.ISET_FQN)
            || type.IsClrType(ClrTypeNames.IReadOnlySet)
            || type.IsClrType(ClrTypeNames.DictionaryKeyCollection))
        {
            return true;
        }

        if (type.GetTypeElement() is { } typeElement)
        {
            var psiModule = context.GetPsiModule();

            return typeElement.IsDescendantOf(PredefinedType.ISET_FQN.TryGetTypeElement(psiModule))
                || typeElement.IsDescendantOf(ClrTypeNames.IReadOnlySet.TryGetTypeElement(psiModule));
        }

        return false;
    }

    [Pure]
    protected static bool IsCollection(IType type, ITreeNode context)
    {
        var psiModule = context.GetPsiModule();

        return type.IsGenericICollection()
            || type.IsGenericIReadOnlyCollection()
            || type.GetTypeElement() is { } typeElement
            && (typeElement.IsDescendantOf(PredefinedType.GENERIC_ICOLLECTION_FQN.TryGetTypeElement(psiModule))
                || typeElement.IsDescendantOf(PredefinedType.GENERIC_IREADONLYCOLLECTION_FQN.TryGetTypeElement(psiModule)));
    }

    public required Func<string, string> Message { get; init; }
}