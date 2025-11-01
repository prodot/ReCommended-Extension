namespace ReCommendedExtension.Extensions.MemberFinding;

internal record struct MethodSignature
{
    public required string Name { get; init; }

    public required IReadOnlyList<Parameter> Parameters { get; init; }

    public bool IsStatic { get; init; }

    [NonNegativeValue]
    public int GenericParametersCount { get; init; }
}