using ReCommendedExtension.Analyzers.ExpressionResult.Inspections;

namespace ReCommendedExtension.Analyzers.ExpressionResult.Rules;

internal sealed record Member
{
    public required MemberSignature Signature { get; init; }

    public required IReadOnlyCollection<Inspection> Inspections { get; init; }
}