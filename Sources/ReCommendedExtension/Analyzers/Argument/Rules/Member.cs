using ReCommendedExtension.Analyzers.Argument.Inspections;

namespace ReCommendedExtension.Analyzers.Argument.Rules;

internal sealed record Member
{
    public required MemberSignature Signature { get; init; }

    public required IReadOnlyCollection<Inspection> Inspections { get; init; }
}