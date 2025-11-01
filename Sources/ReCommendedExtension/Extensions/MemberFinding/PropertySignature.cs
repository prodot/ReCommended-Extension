namespace ReCommendedExtension.Extensions.MemberFinding;

internal record struct PropertySignature
{
    public required string Name { get; init; }

    public bool IsStatic { get; init; }
}