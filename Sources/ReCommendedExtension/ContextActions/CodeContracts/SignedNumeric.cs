using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using ReCommendedExtension.ContextActions.CodeContracts.Internal;

namespace ReCommendedExtension.ContextActions.CodeContracts;

public abstract class SignedNumeric : AddContractContextAction
{
    private protected SignedNumeric(ICSharpContextActionDataProvider provider) : base(provider) { }

    private protected CSharpNumericTypeInfo? NumericTypeInfo { get; private set; }

    [MemberNotNullWhen(true, nameof(NumericTypeInfo))]
    protected sealed override bool IsAvailableForType(IType type)
    {
        NumericTypeInfo = CSharpNumericTypeInfo.TryCreate(type);

        if (NumericTypeInfo is { })
        {
            if (NumericTypeInfo.IsSigned)
            {
                return true;
            }

            NumericTypeInfo = null;
        }

        return false;
    }
}