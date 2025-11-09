using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Extensions.Collections;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal sealed record DuplicateEquivalentCollectionElement : RedundantCollectionElement
{
    public required Func<CollectionCreation, IEnumerable<(IInitializerElement, bool isEquivalent)>> Selector { get; init; }

    public required string MessageEquivalentElement { get; init; }
}