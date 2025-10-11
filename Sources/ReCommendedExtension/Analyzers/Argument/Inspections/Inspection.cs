namespace ReCommendedExtension.Analyzers.Argument.Inspections;

internal abstract record Inspection
{
    public required string Message { get; init; }
}