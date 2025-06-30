namespace ReCommendedExtension.Extensions.MethodFinding;

internal record struct ConstructorSignature
{
    public required IReadOnlyList<ParameterType> ParameterTypes { get; init; }
}