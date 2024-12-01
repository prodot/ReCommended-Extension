using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.Strings.Collections;

internal abstract class CollectionCreation
{
    [Pure]
    public static CollectionCreation? TryFrom(ICSharpExpression? expression)
        => expression switch
        {
            // [...]
            ICollectionExpression collectionExpression => new CollectionExpressionCollectionCreation(collectionExpression),

            // (T[])[...] or (T[]?)[...]
            ICastExpression
            {
                TargetType: IArrayTypeUsage { ArrayRanks: [_] } or INullableTypeUsage { UnderlyingType: IArrayTypeUsage { ArrayRanks: [_] } },
                Op: ICollectionExpression collectionExpression,
            } => new CollectionExpressionCollectionCreation(collectionExpression),

            // new T[] { ... }
            IArrayCreationExpression { DimInits: [], ArrayInitializer: { } } arrayCreationExpression => new ArrayCreationExpressionCollectionCreation(
                arrayCreationExpression),

            // new T[0]
            IArrayCreationExpression
            {
                DimInits: [{ ConstantValue: { Kind: ConstantValueKind.Int, IntValue: 0 } }], ArrayInitializer: not { },
            } arrayCreationExpression => new EmptyCollectionCreation(arrayCreationExpression),

            // default(ReadOnlySpan<T>)
            IDefaultExpression
                {
                    TypeName: IUserTypeUsage { ScalarTypeName: { Reference: var reference, TypeArgumentList.TypeArguments: [_] } },
                } defaultExpression when (reference.Resolve().DeclaredElement as ITypeElement).IsClrType(PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN) =>
                new EmptyCollectionCreation(defaultExpression),

            // new ReadOnlySpan<T>() or new ReadOnlySpan<T> { }
            IObjectCreationExpression
            {
                TypeName: { Reference: var reference, TypeArgumentList.TypeArguments: [_] },
                Arguments: [],
                Initializer: not { } or { InitializerElements: [] },
            } objectCreationExpression when (reference.Resolve().DeclaredElement as ITypeElement).IsClrType(
                PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN) => new EmptyCollectionCreation(objectCreationExpression),

            // (ReadOnlySpan<T>)[...]
            ICastExpression
                {
                    TargetType: IUserTypeUsage { ScalarTypeName: { Reference: var reference, TypeArgumentList.TypeArguments: [_] } },
                    Op: ICollectionExpression collectionExpression,
                } when (reference.Resolve().DeclaredElement as ITypeElement).IsClrType(PredefinedType.SYSTEM_READ_ONLY_SPAN_FQN) =>
                new CollectionExpressionCollectionCreation(collectionExpression),

            // Array.Empty<T>()
            IInvocationExpression { InvokedExpression: IReferenceExpression { Reference: var reference } } invocationExpression when
                reference.Resolve().DeclaredElement is IMethod
                {
                    ShortName: nameof(Array.Empty),
                    IsStatic: true,
                    AccessibilityDomain.DomainType: AccessibilityDomain.AccessibilityDomainType.PUBLIC,
                    TypeParameters: [_],
                    Parameters: [],
                } method
                && method.ContainingType.IsSystemArray() => new EmptyCollectionCreation(invocationExpression),

            _ => null,
        };

    IReadOnlyList<string?>? stringConstants;

    protected abstract IEnumerable<(IInitializerElement, ICSharpExpression)> ElementsWithExpressions { get; }

    public abstract ICSharpExpression Expression { get; }

    [NonNegativeValue]
    public abstract int Count { get; }

    public abstract IEnumerable<IInitializerElement> Elements { get; }

    public abstract IInitializerElement SingleElement { get; }

    public IEnumerable<(IInitializerElement, char)> ElementsWithCharConstants
    {
        get
        {
            foreach (var (element, expression) in ElementsWithExpressions)
            {
                if (expression.TryGetCharConstant() is { } charConstant)
                {
                    yield return (element, charConstant);
                }
            }
        }
    }

    public IEnumerable<(IInitializerElement, string)> ElementsWithStringConstants
    {
        get
        {
            foreach (var (element, expression) in ElementsWithExpressions)
            {
                if (expression.TryGetStringConstant() is { } stringConstant)
                {
                    yield return (element, stringConstant);
                }
            }
        }
    }

    public IReadOnlyList<string?> StringConstants
    {
        get
        {
            if (stringConstants is not { })
            {
                var array = new string?[Count];

                var i = 0;
                foreach (var (_, expression) in ElementsWithExpressions)
                {
                    if (expression.TryGetStringConstant() is { } stringConstant)
                    {
                        array[i++] = stringConstant;
                    }
                }

                stringConstants = [..array];
            }

            return stringConstants;
        }
    }
}