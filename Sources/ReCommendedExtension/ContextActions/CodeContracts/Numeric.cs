using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using ReCommendedExtension.ContextActions.CodeContracts.Internal;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    public abstract class Numeric : AddContractContextAction
    {
        private protected Numeric([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        [CanBeNull]
        private protected CSharpNumericTypeInfo NumericTypeInfo { get; private set; }

        protected sealed override bool IsAvailableForType(IType type)
        {
            NumericTypeInfo = CSharpNumericTypeInfo.TryCreate(type);

            return NumericTypeInfo != null;
        }
    }
}