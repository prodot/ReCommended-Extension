namespace ReCommendedExtension.Analyzers.Method.Inspections;

internal abstract record Inspection
{
    public required Func<string, string> Message { get; init; }
}