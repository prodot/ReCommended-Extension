using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Add contract: Guid is not empty" + ZoneMarker.Suffix,
    Description = "Adds a contract that the Guid is not empty.")]
public sealed class GuidNonEmpty(ICSharpContextActionDataProvider provider) : AddContractContextAction(provider)
{
    protected override bool IsAvailableForType(IType type) => type.IsGuid();

    protected override string GetContractTextForUI(string contractIdentifier) => $"{contractIdentifier} != {nameof(Guid.Empty)}";

    protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
        => factory.CreateExpression(
            $"$0 != $1.{nameof(Guid.Empty)}",
            contractExpression,
            PredefinedType.GUID_FQN.TryGetTypeElement(Provider.PsiModule));
}