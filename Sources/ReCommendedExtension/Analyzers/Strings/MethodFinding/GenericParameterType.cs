using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.Analyzers.Strings.MethodFinding;

internal sealed record GenericParameterType : ParameterType
{
    public override bool IsSameAs(IType otherType, IPsiModule psiModule)
    {
        if (TryGetType(psiModule).IsGenericIEnumerable() && otherType.IsGenericIEnumerable())
        {
            return true;
        }

        return false;
    }
}