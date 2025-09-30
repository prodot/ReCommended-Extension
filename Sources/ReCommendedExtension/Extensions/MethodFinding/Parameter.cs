using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions.MethodFinding;

internal record struct Parameter(Func<IType, bool> IsType, ParameterKind Kind = ParameterKind.VALUE);