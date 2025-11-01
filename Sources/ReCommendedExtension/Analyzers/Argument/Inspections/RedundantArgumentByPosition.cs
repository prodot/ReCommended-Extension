using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Extensions.MemberFinding;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal sealed record RedundantArgumentByPosition : RedundantArgument
{
    public int ParameterIndex { get; init; } = -1;

    public required Func<ICSharpArgument, bool> Condition { get; init; }

    public Func<TreeNodeCollection<ICSharpArgument?>, bool>? FurtherCondition { get; init; }

    public IReadOnlyList<Parameter>? ReplacementSignatureParameters { get; init; }
}