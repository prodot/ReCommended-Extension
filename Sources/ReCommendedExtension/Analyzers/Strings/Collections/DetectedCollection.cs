using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.Analyzers.Strings.Collections;

internal abstract class DetectedCollection
{
    [Pure]
    public static DetectedCollection? TryFrom(ICSharpExpression? expression)
        => expression switch
        {
            // [...]
            ICollectionExpression collectionExpression => new CollectionExpressionDetectedCollection(collectionExpression),

            // (T[])[...] or (T[]?)[...]
            ICastExpression
            {
                TargetType: IArrayTypeUsage { ArrayRanks: [_] } or INullableTypeUsage { UnderlyingType: IArrayTypeUsage { ArrayRanks: [_] } },
                Op: ICollectionExpression collectionExpression,
            } => new CollectionExpressionDetectedCollection(collectionExpression),

            // new T[] { ... }
            IArrayCreationExpression { DimInits: [], ArrayInitializer: { } } arrayCreationExpression => new ArrayCreationExpressionDetectedCollection(
                arrayCreationExpression),

            // new T[0]
            IArrayCreationExpression
            {
                DimInits: [{ ConstantValue: { Kind: ConstantValueKind.Int, IntValue: 0 } }], ArrayInitializer: not { },
            } arrayCreationExpression => new EmptyArrayCreationExpressionDetectedCollection(arrayCreationExpression),

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
                && method.ContainingType.IsSystemArray() => new EmptyArrayCreationExpressionDetectedCollection(invocationExpression),

            _ => null,
        };

    IReadOnlyList<string?>? stringConstants;

    protected abstract IEnumerable<(IInitializerElement, ICSharpExpression)> ElementsWithExpressions { get; }

    public abstract ICSharpExpression Expression { get; }

    [NonNegativeValue]
    public abstract int Count { get; }

    public abstract IEnumerable<IInitializerElement> Elements { get; }

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

                stringConstants = [.. array];

                Debug.Assert(StringConstants.Count == Count);
            }

            return stringConstants;
        }
    }
}