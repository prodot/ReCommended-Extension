using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Add contract: time span is zero" + ZoneMarker.Suffix,
    Description = "Adds a contract that a time span is zero.")]
public sealed class TimeSpanZero(ICSharpContextActionDataProvider provider) : TimeSpan(provider)
{
    protected override string GetContractTextForUI(string contractIdentifier) => $"{contractIdentifier} == {nameof(System.TimeSpan.Zero)}";

    protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
        => factory.CreateExpression(
            $"$0 == $1.{nameof(System.TimeSpan.Zero)}",
            contractExpression,
            PredefinedType.TIMESPAN_FQN.TryGetTypeElement(Provider.PsiModule));
}