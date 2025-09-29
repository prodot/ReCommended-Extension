using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions.MethodFinding;

internal record struct MethodSignature
{
    public required string Name { get; init; }

    public required IReadOnlyList<Func<IType, bool>> ParameterTypes { get; init; }

    public bool IsStatic { get; init; }

    [NonNegativeValue]
    public int GenericParametersCount { get; init; }
}