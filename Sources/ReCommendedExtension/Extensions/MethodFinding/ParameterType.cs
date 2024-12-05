using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.Extensions.MethodFinding;

internal record ParameterType
{
    public required IClrTypeName ClrTypeName { get; init; }

    [Pure]
    protected virtual IType? TryGetType(IPsiModule psiModule)
    {
        if (ClrTypeName.TryGetTypeElement(psiModule) is { } typeElement)
        {
            return TypeFactory.CreateType(typeElement);
        }

        return null;
    }

    [Pure]
    public virtual bool IsSameAs(IType otherType, IPsiModule psiModule)
    {
        if (TryGetType(psiModule) is { } type)
        {
            return TypeEqualityComparer.Default.Equals(type, otherType);
        }

        return false;
    }
}