using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal sealed record ArgumentCondition
{
    public int ParameterIndex { get; init; } = -1;

    public required Func<ICSharpArgument, bool> Condition { get; init; }
}