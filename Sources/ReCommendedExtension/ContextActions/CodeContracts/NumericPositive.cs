using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    GroupType = typeof(CSharpContextActions),
    Name = "Add contract: number is positive" + ZoneMarker.Suffix,
    Description = "Adds a contract that a number is greater than 0.")]
public sealed class NumericPositive(ICSharpContextActionDataProvider provider) : Numeric(provider)
{
    protected override string GetContractTextForUI(string contractIdentifier) => $"{contractIdentifier} > 0";

    protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
    {
        Debug.Assert(NumericTypeInfo is { });

        return factory.CreateExpression($"$0 > 0{NumericTypeInfo.LiteralSuffix}", contractExpression);
    }
}