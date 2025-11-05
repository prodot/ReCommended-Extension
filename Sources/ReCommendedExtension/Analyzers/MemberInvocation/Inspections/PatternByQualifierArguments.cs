using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.MemberInvocation.Rules;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record PatternByQualifierArguments : Pattern
{
    public required Func<ICSharpExpression, TreeNodeCollection<ICSharpArgument?>, PatternReplacement?> TryGetReplacement { get; init; }

    public bool EnsureQualifierNotNull { get; init; }

    public bool EnsureExtensionInvokedAsExtension { get; init; }

    public bool EnsureNoTypeArguments { get; init; }
}