using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal sealed record DuplicateArgument : RedundantArgument
{
    public required Func<TreeNodeCollection<ICSharpArgument?>, IEnumerable<ICSharpArgument>> Selector { get; init; }
}