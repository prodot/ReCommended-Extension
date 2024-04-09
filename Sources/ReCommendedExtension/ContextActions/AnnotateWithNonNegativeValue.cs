using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions
{
    [ContextAction(
        GroupType = typeof(CSharpContextActions),
        Name = "Annotate with [NonNegativeValue] attribute" + ZoneMarker.Suffix,
        Description = "Annotates with the [NonNegativeValue] attribute.")]
    public sealed class AnnotateWithNonNegativeValue : AnnotateWithCodeAnnotation
    {
        [Pure]
        static bool IsAvailableForType([NotNull] IType type) => type.IsInt() || type.IsLong() || type.IsShort() || type.IsSbyte();

        public AnnotateWithNonNegativeValue([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override string AnnotationAttributeTypeName => nameof(NonNegativeValueAttribute);

        protected override bool CanBeAnnotated(IDeclaredElement declaredElement, ITreeNode context)
        {
            switch (declaredElement)
            {
                case IMethod method: return IsAvailableForType(method.ReturnType);

                case IProperty property: return IsAvailableForType(property.Type);

                case IField field when !field.IsConstant: return IsAvailableForType(field.Type);

                case IParameter parameter: return IsAvailableForType(parameter.Type);

                case IDelegate delegateType: return IsAvailableForType(delegateType.InvokeMethod.ReturnType);
            }

            return false;
        }
    }
}