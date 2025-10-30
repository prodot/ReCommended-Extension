using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Rules;

public record struct PatternReplacement
{
    public required ICSharpExpression Expression { get; init; }

    public required string Pattern { get; init; }
}