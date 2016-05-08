using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Impl.Types;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    [ContextAction(Group = "C#", Name = "Add contract: date/time doesn't have a time part" + ZoneMarker.Suffix,
        Description = "Adds a contract that the date/time doesn't have a time part.")]
    public sealed class DateTimeTimeOfDayZero : AddContractContextAction
    {
        public DateTimeTimeOfDayZero([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override bool IsAvailableForType(IType type) => type.IsDateTime();

        protected override string GetContractTextForUI(string contractIdentifier)
            => string.Format("{0}.{1} == {2}", contractIdentifier, nameof(DateTime.TimeOfDay), nameof(System.TimeSpan.Zero));

        protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
            =>
                factory.CreateExpression(
                    string.Format("$0.{0} == $1.{1}", nameof(DateTime.TimeOfDay), nameof(System.TimeSpan.Zero)),
                    contractExpression,
                    new DeclaredTypeFromCLRName(ClrTypeNames.TimeSpan, Provider.PsiModule).GetTypeElement());
    }
}