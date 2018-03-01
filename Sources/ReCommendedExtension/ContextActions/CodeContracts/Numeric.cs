using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using ReCommendedExtension.ContextActions.CodeContracts.Internal;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    public abstract class Numeric : AddContractContextAction
    {
        internal Numeric([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected sealed override bool IsAvailableForType(IType type)
        {
            NumericTypeInfo = CSharpNumericTypeInfo.TryCreate(type);

            return NumericTypeInfo != null;
        }

        internal CSharpNumericTypeInfo NumericTypeInfo { get; private set; }
    }
}