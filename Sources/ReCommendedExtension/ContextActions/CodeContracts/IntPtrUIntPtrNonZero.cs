using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Impl.Types;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    [ContextAction(Group = "C#", Name = "Add contract: IntPtr is not zero" + ZoneMarker.Suffix,
        Description = "Adds a contract that the IntPtr (or UIntPtr) value is not zero.")]
    public sealed class IntPtrUIntPtrNonZero : IntPtrUIntPtr
    {
        public IntPtrUIntPtrNonZero([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override string GetContractTextForUI(string contractIdentifier)
            => string.Format("{0} != {1}", contractIdentifier, nameof(IntPtr.Zero));

        protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
        {
            Debug.Assert(Provider.PsiModule != null);
            Debug.Assert(nameof(IntPtr.Zero) == nameof(UIntPtr.Zero));

            return factory.CreateExpression(
                string.Format("$0 != $1.{0}", nameof(IntPtr.Zero)),
                contractExpression,
                new DeclaredTypeFromCLRName(IsSigned ? PredefinedType.INTPTR_FQN : PredefinedType.UINTPTR_FQN, Provider.PsiModule).GetTypeElement());
        }
    }
}