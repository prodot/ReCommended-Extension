using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Extensions.Collections;

internal sealed class CollectionExpressionCollectionCreation(ICollectionExpression collectionExpression) : CollectionCreation
{
    protected override IEnumerable<(IInitializerElement, ICSharpExpression)> ElementsWithExpressions
    {
        get
        {
            foreach (var element in collectionExpression.CollectionElements)
            {
                if (element is IExpressionElement expressionElement)
                {
                    yield return (element, expressionElement.Expression);
                }
            }
        }
    }

    public override ICSharpExpression Expression => collectionExpression;

    public override int Count => collectionExpression.CollectionElements.Count;

    public override ICSharpExpression? SingleExpressionElement
        => collectionExpression.CollectionElements is [IExpressionElement element] ? element.Expression : null;
}