using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions;

internal static class TypeElementExtensions
{
    extension(ITypeElement type)
    {
        public bool IsDisposable
            => type.IsClrType(PredefinedType.IDISPOSABLE_FQN)
                || type.IsClrType(PredefinedType.IASYNCDISPOSABLE_FQN)
                || type.IsDescendantOf(PredefinedType.IDISPOSABLE_FQN.TryGetTypeElement(type.Module))
                || type.IsDescendantOf(PredefinedType.IASYNCDISPOSABLE_FQN.TryGetTypeElement(type.Module));
    }
}