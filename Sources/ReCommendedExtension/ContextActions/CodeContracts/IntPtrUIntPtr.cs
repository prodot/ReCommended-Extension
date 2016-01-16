using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    public abstract class IntPtrUIntPtr : AddContractContextAction
    {
        internal IntPtrUIntPtr([NotNull] ICSharpContextActionDataProvider provider) : base(provider) {}

        protected sealed override bool IsAvailableForType(IType type)
        {
            if (TypesUtil.IsPredefinedTypeFromAssembly(type, PredefinedType.INTPTR_FQN, assembly => assembly.AssertNotNull().IsMscorlib))
            {
                IsSigned = true;
                return true;
            }

            if (TypesUtil.IsPredefinedTypeFromAssembly(type, PredefinedType.UINTPTR_FQN, assembly => assembly.AssertNotNull().IsMscorlib))
            {
                IsSigned = false;
                return true;
            }

            return false;
        }

        internal bool IsSigned { get; private set; }
    }
}