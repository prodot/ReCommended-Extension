using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    public abstract class TimeSpan : AddContractContextAction
    {
        private protected TimeSpan([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected sealed override bool IsAvailableForType(IType type) => type.IsTimeSpan();
    }
}