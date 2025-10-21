using ReCommendedExtension.Analyzers.Method.Rules;

namespace ReCommendedExtension.Analyzers.Method;

public record struct ReplacedMethodInvocation
{
    public required string Name { get; init; }

    public required InvocationReplacement Replacement { get; init; }
}