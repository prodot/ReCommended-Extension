using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions;

internal static class TypeElementExtensions
{
    [Pure]
    public static bool IsDisposable(this ITypeElement type)
        => type.IsClrType(PredefinedType.IDISPOSABLE_FQN)
            || type.IsClrType(PredefinedType.IASYNCDISPOSABLE_FQN)
            || type.IsDescendantOf(PredefinedType.IDISPOSABLE_FQN.TryGetTypeElement(type.Module))
            || type.IsDescendantOf(PredefinedType.IASYNCDISPOSABLE_FQN.TryGetTypeElement(type.Module));
}