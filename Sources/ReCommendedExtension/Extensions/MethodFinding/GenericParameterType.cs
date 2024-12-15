using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.Extensions.MethodFinding;

internal sealed record GenericParameterType : ParameterType
{
    public override bool IsSameAs(IType otherType, IPsiModule psiModule)
    {
        if (TryGetType(psiModule).IsGenericIEnumerable() && otherType.IsGenericIEnumerable())
        {
            return true;
        }

        if (TryGetType(psiModule).IsReadOnlySpan() && otherType.IsReadOnlySpan())
        {
            return true; // span type argument intentionally ignored (sufficient for now)
        }

        return false;
    }
}