using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.ContextActions.CodeContracts;

public abstract class TimeSpan(ICSharpContextActionDataProvider provider) : AddContractContextAction(provider)
{
    protected sealed override bool IsAvailableForType(IType type) => type.IsTimeSpan();
}