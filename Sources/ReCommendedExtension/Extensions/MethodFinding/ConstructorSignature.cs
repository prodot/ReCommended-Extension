using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions.MethodFinding;

internal record struct ConstructorSignature
{
    public required IReadOnlyList<Func<IType, bool>> ParameterTypes { get; init; }
}