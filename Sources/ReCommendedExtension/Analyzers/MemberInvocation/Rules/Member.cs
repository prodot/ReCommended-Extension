using ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Rules;

internal sealed record Member
{
    public required MemberSignature Signature { get; init; }

    public required IReadOnlyCollection<Inspection> Inspections { get; init; }
}