using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    [ContextAction(
        Group = "C#",
        Name = "Add contract: number is zero or negative" + ZoneMarker.Suffix,
        Description = "Adds a contract that a number (signed) is less than or equal to 0.")]
    public sealed class SignedNumericZeroOrNegative : SignedNumeric
    {
        public SignedNumericZeroOrNegative([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override string GetContractTextForUI(string contractIdentifier) => $"{contractIdentifier} <= 0";

        protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
        {
            Debug.Assert(NumericTypeInfo != null);

            return factory.CreateExpression($"$0 <= 0{NumericTypeInfo.LiteralSuffix}", contractExpression);
        }
    }
}