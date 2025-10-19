using ReCommendedExtension.Extensions.MethodFinding;

namespace ReCommendedExtension.Analyzers.Method.Rules;

internal sealed record MethodSignature
{
    public required IReadOnlyList<Parameter> Parameters { get; init; }

    public bool IsStatic { get; init; }

    [NonNegativeValue]
    public int GenericParametersCount { get; init; }
}