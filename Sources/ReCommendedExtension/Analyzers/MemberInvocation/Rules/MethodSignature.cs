using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Rules;

internal sealed record MethodSignature : MemberSignature
{
    public required IReadOnlyList<Parameter> Parameters { get; init; }

    [NonNegativeValue]
    public int GenericParametersCount { get; init; }
}