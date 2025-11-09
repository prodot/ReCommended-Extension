using ReCommendedExtension.Analyzers.MemberInvocation.Rules;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

public record struct ReplacedMethodInvocation
{
    public required string Name { get; init; }

    public required InvocationReplacement Replacement { get; init; }
}