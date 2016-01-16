using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Impl.Types;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    [ContextAction(Group = "C#", Name = "Add contract: Guid is not empty" + ZoneMarker.Suffix,
        Description = "Adds a contract that the Guid is not empty.")]
    public sealed class GuidNonEmpty : AddContractContextAction
    {
        public GuidNonEmpty([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override bool IsAvailableForType(IType type)
            => TypesUtil.IsPredefinedTypeFromAssembly(type, ClrTypeNames.Guid, assembly => assembly.AssertNotNull().IsMscorlib);

        protected override string GetContractTextForUI(string contractIdentifier)
            => string.Format("{0} != {1}", contractIdentifier, nameof(Guid.Empty));

        protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
        {
            Debug.Assert(Provider.PsiModule != null);

            return factory.CreateExpression(
                string.Format("$0 != $1.{0}", nameof(Guid.Empty)),
                contractExpression,
                new DeclaredTypeFromCLRName(ClrTypeNames.Guid, Provider.PsiModule).GetTypeElement());
        }
    }
}