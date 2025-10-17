using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Analyzers.Argument;

public readonly record struct UpcomingArgument
{
    public required ParameterKind ParameterKind { get; init; }

    public string? ParameterName { get; init; }

    public required string Value { get; init; }
}