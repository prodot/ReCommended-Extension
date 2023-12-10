using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    Group = "C#",
    Name = "Add contract: number is zero or positive" + ZoneMarker.Suffix,
    Description = "Adds a contract that a number (signed) is greater than or equal to 0.")]
public sealed class SignedNumericZeroOrPositive(ICSharpContextActionDataProvider provider) : SignedNumeric(provider)
{
    protected override string GetContractTextForUI(string contractIdentifier) => $"{contractIdentifier} >= 0";

    protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
    {
        Debug.Assert(NumericTypeInfo is { });

        return factory.CreateExpression($"$0 >= 0{NumericTypeInfo.LiteralSuffix}", contractExpression);
    }
}