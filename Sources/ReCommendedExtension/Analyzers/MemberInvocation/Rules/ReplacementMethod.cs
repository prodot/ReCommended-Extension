using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Rules;

internal sealed record ReplacementMethod
{
    public required string Name { get; init; }

    public required IReadOnlyList<Parameter> Parameters { get; init; }

    [NonNegativeValue]
    public int GenericParametersCount { get; init; }
}