using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Argument;

public readonly record struct ArgumentReplacement
{
    public required ICSharpArgument Argument { get; init; }

    public required UpcomingArgument Replacement { get; init; }
}