using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Modules;

namespace ReCommendedExtension.Extensions.MethodFinding;

internal sealed record ArrayParameterType : ParameterType
{
    protected override IType? TryGetType(IPsiModule psiModule)
    {
        if (base.TryGetType(psiModule) is { } type)
        {
            return TypeFactory.CreateArrayType(type, 1);
        }

        return null;
    }
}