using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Add contract: IntPtr is not zero" + ZoneMarker.Suffix,
    Description = "Adds a contract that the IntPtr (or UIntPtr) value is not zero.")]
public sealed class IntPtrUIntPtrNonZero(ICSharpContextActionDataProvider provider) : IntPtrUIntPtr(provider)
{
    protected override string GetContractTextForUI(string contractIdentifier) => $"{contractIdentifier} != {nameof(IntPtr.Zero)}";

    protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
    {
        Debug.Assert(nameof(IntPtr.Zero) == nameof(UIntPtr.Zero));

        return factory.CreateExpression(
            $"$0 != $1.{nameof(IntPtr.Zero)}",
            contractExpression,
            (IsSigned ? PredefinedType.INTPTR_FQN : PredefinedType.UINTPTR_FQN).TryGetTypeElement(Provider.PsiModule));
    }
}