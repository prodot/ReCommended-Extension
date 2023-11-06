using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.ContextActions.CodeContracts;

public abstract class TimeSpan : AddContractContextAction
{
    private protected TimeSpan(ICSharpContextActionDataProvider provider) : base(provider) { }

    protected sealed override bool IsAvailableForType(IType type) => type.IsTimeSpan();
}