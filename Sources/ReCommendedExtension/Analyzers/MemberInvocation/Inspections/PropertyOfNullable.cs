using ReCommendedExtension.Analyzers.MemberInvocation.Rules;

namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record PropertyOfNullable : Inspection
{
    public static PropertyOfNullable HasValue { get; } =
        new() { Name = PropertyNameOfNullable.HasValue, Message = _ => "Use pattern or null check." };

    public static PropertyOfNullable Value { get; } =
        new() { Name = PropertyNameOfNullable.Value, EnsureQualifierNotValueTuple = true, Message = _ => "Use type cast." };

    public bool EnsureQualifierNotValueTuple { get; private init; }

    public required PropertyNameOfNullable Name { get; init; }
}