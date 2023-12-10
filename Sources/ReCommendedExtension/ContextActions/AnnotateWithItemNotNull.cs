using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Util;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Annotate with [ItemNotNull] attribute" + ZoneMarker.Suffix,
    Description = "Annotates with the [ItemNotNull] attribute.")]
public sealed class AnnotateWithItemNotNull(ICSharpContextActionDataProvider provider) : AnnotateWithCodeAnnotation(provider)
{
    static bool IsAvailableForType(IType type, ITreeNode context)
    {
        if ((type.IsGenericEnumerableOrDescendant() || type.IsGenericArray(context))
            && CollectionTypeUtil.ElementTypeByCollectionType(type, context, false) is { Classify: TypeClassification.REFERENCE_TYPE })
        {
            return true;
        }

        if (type.GetTasklikeUnderlyingType(context) is { Classify: TypeClassification.REFERENCE_TYPE })
        {
            return true;
        }

        if (type.IsLazy())
        {
            var typeElement = TypeElementUtil.GetTypeElementByClrName(PredefinedType.LAZY_FQN, context.GetPsiModule());
            if (type.GetGenericUnderlyingType(typeElement) is { Classify: TypeClassification.REFERENCE_TYPE })
            {
                return true;
            }
        }

        return false;
    }

    protected override string AnnotationAttributeTypeName => nameof(ItemNotNullAttribute);

    protected override bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context)
    {
        if (context.IsNullableAnnotationsContextEnabled())
        {
            return false;
        }

        return declaredElement switch
        {
            IMethod method => IsAvailableForType(method.ReturnType, context),
            IParameter parameter => IsAvailableForType(parameter.Type, context),
            IProperty property => IsAvailableForType(property.Type, context),
            IDelegate delegateType => IsAvailableForType(delegateType.InvokeMethod.ReturnType, context),
            IField field => IsAvailableForType(field.Type, context),

            _ => false,
        };
    }
}