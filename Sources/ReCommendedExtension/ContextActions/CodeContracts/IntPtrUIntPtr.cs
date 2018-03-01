using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Impl;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    public abstract class IntPtrUIntPtr : AddContractContextAction
    {
        internal IntPtrUIntPtr([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected sealed override bool IsAvailableForType(IType type)
        {
            if (type is IDeclaredType declaredType)
            {
                var typeElement = declaredType.GetTypeElement();

                if (typeElement != null)
                {
                    if (DeclaredElementEqualityComparer.TypeElementComparer.Equals(
                        typeElement,
                        typeElement.Module.GetPredefinedType().TryGetType(PredefinedType.INTPTR_FQN)?.GetTypeElement()))
                    {
                        IsSigned = true;
                        return true;
                    }

                    if (DeclaredElementEqualityComparer.TypeElementComparer.Equals(
                        typeElement,
                        typeElement.Module.GetPredefinedType().TryGetType(PredefinedType.UINTPTR_FQN)?.GetTypeElement()))
                    {
                        IsSigned = false;
                        return true;
                    }
                }
            }

            return false;
        }

        internal bool IsSigned { get; private set; }
    }
}