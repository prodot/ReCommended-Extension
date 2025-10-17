using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes.Collections;

internal sealed class EmptyCollectionCreation(ICSharpExpression expression) : CollectionCreation
{
    protected override IEnumerable<(IInitializerElement, ICSharpExpression)> ElementsWithExpressions
    {
        get
        {
            yield break;
        }
    }

    public override ICSharpExpression Expression => expression;

    public override int Count => 0;

    public override IInitializerElement SingleElement => throw new NotSupportedException();
}