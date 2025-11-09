namespace ReCommendedExtension.Extensions.MemberFinding;

internal record struct ConstructorSignature
{
    public required IReadOnlyList<Parameter> Parameters { get; init; }
}