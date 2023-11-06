using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts;

[ContextAction(
    Group = "C#",
    Name = "Add contract: number is not 0" + ZoneMarker.Suffix,
    Description = "Adds a contract that a number (signed) is not 0.")]
public sealed class SignedNumericNonZero : SignedNumeric
{
    public SignedNumericNonZero(ICSharpContextActionDataProvider provider) : base(provider) { }

    protected override string GetContractTextForUI(string contractIdentifier) => $"{contractIdentifier} != 0";

    protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
    {
        Debug.Assert(NumericTypeInfo is { });

        return NumericTypeInfo.EpsilonLiteral is { }
            ? factory.CreateExpression(
                $"$1.{nameof(Math.Abs)}($0 - 0{NumericTypeInfo.LiteralSuffix}) > {NumericTypeInfo.EpsilonLiteral}",
                contractExpression,
                TypeElementUtil.GetTypeElementByClrName(ClrTypeNames.Math, Provider.PsiModule))
            : factory.CreateExpression($"$0 != 0{NumericTypeInfo.LiteralSuffix}", contractExpression);
    }
}