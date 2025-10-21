using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Method.Rules;

public record struct InvocationReplacement
{
    public required ICSharpTreeNode OriginalExpression { get; init; }

    public required MethodInvocationContext Context { get; init; }

    public required IReadOnlyList<string> Arguments { get; init; }

    public required bool IsNegated { get; init; }
}