using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.MemberInvocation.Rules;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record PatternByBinaryExpression : Pattern
{
    public required Func<IInvocationExpression, ICSharpExpression, TreeNodeCollection<ICSharpArgument?>, PatternReplacement?> TryGetReplacement { get; init; }

    public bool EnsureQualifierNotNull { get; init; }
}