namespace ReCommendedExtension.Analyzers.ExpressionResult;

public record struct ExpressionResultReplacements
{
    public required string Main { get; init; }

    public string? Alternative { get; init; }
}