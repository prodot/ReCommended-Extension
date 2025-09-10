using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.Extensions.MethodFinding;

internal sealed record GenericParameterType : ParameterType
{
    public override bool IsSameAs(IType otherType, IPsiModule psiModule)
    {
        var type = TryGetType(psiModule);

        if (type.IsGenericIEnumerable() && otherType.IsGenericIEnumerable())
        {
            return true; // collection type argument intentionally ignored (sufficient for now)
        }

        if (type.IsReadOnlySpan() && otherType.IsReadOnlySpan())
        {
            return true; // span type argument intentionally ignored (sufficient for now)
        }

        return false;
    }
}