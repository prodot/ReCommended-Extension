using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.Extensions;

internal static class TypeElementExtensions
{
    [Pure]
    public static bool IsDisposable(this ITypeElement type, IPsiModule psiModule)
        => type.IsClrType(PredefinedType.IDISPOSABLE_FQN)
            || type.IsClrType(PredefinedType.IASYNCDISPOSABLE_FQN)
            || type.IsDescendantOf(PredefinedType.IDISPOSABLE_FQN.TryGetTypeElement(psiModule))
            || type.IsDescendantOf(PredefinedType.IASYNCDISPOSABLE_FQN.TryGetTypeElement(psiModule));
}