using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    [ContextAction(
        GroupType = typeof(CSharpContextActions),
        Name = "Add contract: a nullable value is not null" + ZoneMarker.Suffix,
        Description = "Adds a contract that a nullable value is not null.")]
    public sealed class NotNull : AddContractContextAction
    {
        public NotNull([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override bool IsAvailableForType(IType type) => type.Classify == TypeClassification.REFERENCE_TYPE;

        protected override string GetContractTextForUI(string contractIdentifier) => $"{contractIdentifier} != null";

        protected override IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression)
            => factory.CreateExpression("$0 != null", contractExpression);

        protected override string TryGetAnnotationAttributeTypeName() => NullnessProvider.NotNullAttributeShortName;
    }
}