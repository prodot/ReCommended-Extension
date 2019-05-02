using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Modules;
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
                var elementType = CollectionTypeUtil.ElementTypeByCollectionType(type, context, false);
                if (elementType != null && elementType.Classify == TypeClassification.REFERENCE_TYPE)
                {
                    return true;
                }
            }

            var resultType = type.GetTasklikeUnderlyingType(context);
            if (resultType != null && resultType.Classify == TypeClassification.REFERENCE_TYPE)
            {
                return true;
            }

            if (type.IsLazy())
            {
                var typeElement = TypeElementUtil.GetTypeElementByClrName(PredefinedType.LAZY_FQN, context.GetPsiModule());
                var valueType = type.GetGenericUnderlyingType(typeElement);
                if (valueType != null && valueType.Classify == TypeClassification.REFERENCE_TYPE)
                {
                    return true;
                }
            }

            return false;
        }

        public AnnotateWithItemNotNull([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected override string AnnotationAttributeTypeName
        {
            get
            {
                Debug.Assert(ContainerElementNullnessProvider.ItemNotNullAttributeShortName != null);

                return ContainerElementNullnessProvider.ItemNotNullAttributeShortName;
            }
        }

        protected override bool CanBeAnnotated(IDeclaredElement declaredElement, ITreeNode context, IPsiModule psiModule)
        {
            switch (declaredElement)
            {
                case IMethod method when IsAvailableForType(method.ReturnType, context): return true;

                case IParameter parameter when IsAvailableForType(parameter.Type, context): return true;

                case IProperty property when IsAvailableForType(property.Type, context): return true;

                case IDelegate delegateType when IsAvailableForType(delegateType.InvokeMethod.ReturnType, context): return true;

                case IField field when IsAvailableForType(field.Type, context): return true;

                default: return false;
            }
        }
    }
}