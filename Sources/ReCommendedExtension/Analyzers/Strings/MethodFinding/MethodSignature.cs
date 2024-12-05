namespace ReCommendedExtension.Analyzers.Strings.MethodFinding;

internal record struct MethodSignature
{
    public required string Name { get; init; }

    public required IReadOnlyList<ParameterType> ParameterTypes { get; init; }

    public bool IsStatic { get; init; }

    [NonNegativeValue]
    public int GenericParametersCount { get; init; }
}