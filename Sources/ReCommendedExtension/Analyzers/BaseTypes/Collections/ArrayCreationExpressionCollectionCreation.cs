using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes.Collections;

internal sealed class ArrayCreationExpressionCollectionCreation : CollectionCreation
{
    readonly IArrayCreationExpression arrayCreationExpression;

    internal ArrayCreationExpressionCollectionCreation(IArrayCreationExpression arrayCreationExpression)
    {
        Debug.Assert(arrayCreationExpression.ArrayInitializer is { });

        this.arrayCreationExpression = arrayCreationExpression;
    }

    protected override IEnumerable<(IInitializerElement, ICSharpExpression)> ElementsWithExpressions
    {
        get
        {
            foreach (var element in arrayCreationExpression.ArrayInitializer.ElementInitializers)
            {
                if (element is IExpressionInitializer elementInitializer)
                {
                    yield return (element, elementInitializer.Value);
                }
            }
        }
    }

    public override ICSharpExpression Expression => arrayCreationExpression;

    public override int Count => arrayCreationExpression.ArrayInitializer.ElementInitializers.Count;

    public override IEnumerable<IInitializerElement> Elements => arrayCreationExpression.ArrayInitializer.ElementInitializers;

    public override IInitializerElement SingleElement
    {
        get
        {
            Debug.Assert(Count == 1);

            return arrayCreationExpression.ArrayInitializer.ElementInitializers[0];
        }
    }
}