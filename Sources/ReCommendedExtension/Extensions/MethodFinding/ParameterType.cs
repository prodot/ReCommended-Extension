using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.Extensions.MethodFinding;

internal record ParameterType
{
    public IClrTypeName? ClrTypeName { get; init; }

    [Pure]
    protected virtual IType? TryGetType(IPsiModule psiModule)
    {
        if (ClrTypeName is { })
        {
            if (ClrTypeName.TryGetTypeElement(psiModule) is { } typeElement)
            {
                return TypeFactory.CreateType(typeElement);
            }

            return null;
        }

        return TypeFactory.CreateUnknownType(psiModule);
    }

    [Pure]
    public virtual bool IsSameAs(IType otherType, IPsiModule psiModule)
    {
        if (TryGetType(psiModule) is { } type)
        {
            return type.IsUnknown
                ? TypeEqualityComparer.DefaultOrUnknown.Equals(type, otherType)
                : TypeEqualityComparer.Default.Equals(type, otherType);
        }

        return false;
    }
}