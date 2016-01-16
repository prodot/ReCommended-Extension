using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using ReCommendedExtension.ContextActions.CodeContracts.Internal;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    public abstract class SignedNumeric : AddContractContextAction
    {
        internal SignedNumeric([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected sealed override bool IsAvailableForType(IType type)
        {
            NumericTypeInfo = CSharpNumericTypeInfo.TryCreate(type);

            if (NumericTypeInfo != null)
            {
                if (NumericTypeInfo.IsSigned)
                {
                    return true;
                }

                NumericTypeInfo = null;
            }

            return false;
        }

        internal CSharpNumericTypeInfo NumericTypeInfo { get; private set; }
    }
}