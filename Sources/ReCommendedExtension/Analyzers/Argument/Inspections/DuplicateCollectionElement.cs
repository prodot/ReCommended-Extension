using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.BaseTypes.Collections;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal sealed record DuplicateCollectionElement : RedundantCollectionElement
{
    public required Func<CollectionCreation, IEnumerable<IInitializerElement>> Selector { get; init; }
}