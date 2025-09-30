namespace ReCommendedExtension.Extensions.MethodFinding;

internal record struct ConstructorSignature
{
    public required IReadOnlyList<Parameter> Parameters { get; init; }
}