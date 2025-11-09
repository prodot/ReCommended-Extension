namespace ReCommendedExtension.Analyzers.MemberInvocation.Inspections;

internal sealed record PatternByArgument : Pattern
{
    public int ParameterIndex { get; init; } = -1;

    public string? Pattern { get; init; }
}