using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.MemberInvocation.Rules;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record PatternByArguments : Pattern
{
    public required Func<TreeNodeCollection<ICSharpArgument?>, PatternReplacement?> TryGetReplacement { get; init; }
}