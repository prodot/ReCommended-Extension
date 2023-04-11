using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions
{
    [ContextAction(
        Group = "C#",
        Name = "Annotate with [ItemNotNull] attribute" + ZoneMarker.Suffix,
        Description = "Annotates with the [ItemNotNull] attribute.")]
    public sealed class AnnotateWithItemNotNull : AnnotateWithCodeAnnotation
    {
        static bool IsAvailableForType([NotNull] IType type, [NotNull] ITreeNode context)
        {
            if (type.IsGenericEnumerableOrDescendant() || type.IsGenericArray(context))
            {
                if (CollectionTypeUtil.ElementTypeByCollectionType(type, context, false)?.Classify == TypeClassification.REFERENCE_TYPE)
                {
                    return true;
                }
            }

            if (type.GetTasklikeUnderlyingType(context)?.Classify == TypeClassification.REFERENCE_TYPE)
            {
                return true;
            }

            if (type.IsLazy())
            {
                var typeElement = TypeElementUtil.GetTypeElementByClrName(PredefinedType.LAZY_FQN, context.GetPsiModule());
                if (type.GetGenericUnderlyingType(typeElement)?.Classify == TypeClassification.REFERENCE_TYPE)
                {
                    return true;
                }
            }

            return false;
        }

        public AnnotateWithItemNotNull([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override string AnnotationAttributeTypeName => nameof(ItemNotNullAttribute);

        protected override bool CanBeAnnotated(IDeclaredElement declaredElement, ITreeNode context)
        {
            if (context.IsNullableAnnotationsContextEnabled())
            {
                return false;
            }

            switch (declaredElement)
            {
                case IMethod method: return IsAvailableForType(method.ReturnType, context);

                case IParameter parameter: return IsAvailableForType(parameter.Type, context);

                case IProperty property: return IsAvailableForType(property.Type, context);

                case IDelegate delegateType: return IsAvailableForType(delegateType.InvokeMethod.ReturnType, context);

                case IField field: return IsAvailableForType(field.Type, context);
            }

            return false;
        }
    }
}