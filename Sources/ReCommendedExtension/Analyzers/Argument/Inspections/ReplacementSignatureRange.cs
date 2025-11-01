using ReCommendedExtension.Extensions.MemberFinding;

namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal sealed record ReplacementSignatureRange
{
    public required IReadOnlyList<Parameter> Parameters { get; init; }

    public required Range ParameterIndexRange { get; init; }
}