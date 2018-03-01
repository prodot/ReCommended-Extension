using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    [ContextAction(
        Group = "C#",
        Name = "Add contract: Guid is not empty" + ZoneMarker.Suffix,
        Description = "Adds a contract that the Guid is not empty.")]
    public sealed class GuidNonEmpty : AddContractContextAction
    {
        public GuidNonEmpty([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override bool IsAvailableForType(IType type) => type.IsGuid();

        protected override string GetContractTextForUI(string contractIdentifier) => $"{contractIdentifier} != {nameof(Guid.Empty)}";

        protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
            => factory.CreateExpression(
                string.Format("$0 != $1.{0}", nameof(Guid.Empty)),
                contractExpression,
                TypeElementUtil.GetTypeElementByClrName(PredefinedType.GUID_FQN, Provider.PsiModule));
    }
}