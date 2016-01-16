using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    public abstract class TimeSpan : AddContractContextAction
    {
        internal TimeSpan([NotNull] ICSharpContextActionDataProvider provider) : base(provider) {}

        protected sealed override bool IsAvailableForType(IType type)
            => TypesUtil.IsPredefinedTypeFromAssembly(type, ClrTypeNames.TimeSpan, assembly => assembly.AssertNotNull().IsMscorlib);
    }
}