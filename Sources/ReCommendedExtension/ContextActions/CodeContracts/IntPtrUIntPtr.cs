using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;

namespace ReCommendedExtension.ContextActions.CodeContracts;

public abstract class IntPtrUIntPtr(ICSharpContextActionDataProvider provider) : AddContractContextAction(provider)
{
    private protected bool IsSigned { get; private set; }

    protected sealed override bool IsAvailableForType(IType type)
    {
        if (type is IDeclaredType declaredType && declaredType.GetTypeElement() is { } typeElement)
        {
            if (DeclaredElementEqualityComparer.TypeElementComparer.Equals(
                typeElement,
                typeElement.Module.GetPredefinedType().TryGetType(PredefinedType.INTPTR_FQN, NullableAnnotation.Unknown)?.GetTypeElement()))
            {
                IsSigned = true;
                return true;
            }

            if (DeclaredElementEqualityComparer.TypeElementComparer.Equals(
                typeElement,
                typeElement.Module.GetPredefinedType().TryGetType(PredefinedType.UINTPTR_FQN, NullableAnnotation.Unknown)?.GetTypeElement()))
            {
                IsSigned = false;
                return true;
            }
        }

        return false;
    }
}