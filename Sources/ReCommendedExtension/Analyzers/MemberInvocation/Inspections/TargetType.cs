using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

public sealed record TargetType
{
    internal static TargetType Long { get; } = new() { Name = "long", IsType = type => type.IsLong(), IsNullableType = type => type.IsNullableLong() };

    public required string Name { get; init; }

    public required Func<IType?, bool> IsType { get; init; }

    public required Func<IType?, bool> IsNullableType { get; init; }
}