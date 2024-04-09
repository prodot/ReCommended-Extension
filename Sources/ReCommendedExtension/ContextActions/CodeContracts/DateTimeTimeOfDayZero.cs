using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    [ContextAction(
        GroupType = typeof(CSharpContextActions),
        Name = "Add contract: date/time doesn't have a time part" + ZoneMarker.Suffix,
        Description = "Adds a contract that the date/time doesn't have a time part.")]
    public sealed class DateTimeTimeOfDayZero : AddContractContextAction
    {
        public DateTimeTimeOfDayZero([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override bool IsAvailableForType(IType type) => type.IsDateTime();

        protected override string GetContractTextForUI(string contractIdentifier)
            => $"{contractIdentifier}.{nameof(DateTime.TimeOfDay)} == {nameof(System.TimeSpan.Zero)}";

        protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
            => factory.CreateExpression(
                $"$0.{nameof(DateTime.TimeOfDay)} == $1.{nameof(System.TimeSpan.Zero)}",
                contractExpression,
                TypeElementUtil.GetTypeElementByClrName(PredefinedType.TIMESPAN_FQN, Provider.PsiModule));
    }
}