using ReCommendedExtension.Analyzers.Method.Inspections;

namespace ReCommendedExtension.Analyzers.Method.Rules;

internal sealed record Method
{
    public required MethodSignature Signature { get; init; }

    public required IReadOnlyCollection<Inspection> Inspections { get; init; }
}