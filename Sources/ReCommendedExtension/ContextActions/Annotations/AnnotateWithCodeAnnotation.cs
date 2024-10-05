using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions.Annotations;

public abstract class AnnotateWithCodeAnnotation(ICSharpContextActionDataProvider provider) : AnnotateWith(provider)
{
    protected override bool IsAttribute(IAttribute attribute) => attribute.GetAttributeType().GetClrName().ShortName == AnnotationAttributeTypeName;

    protected sealed override Func<CSharpElementFactory, IAttribute>? CreateAttributeFactoryIfAvailable(
        IAttributesOwnerDeclaration attributesOwnerDeclaration,
        out IAttribute[] attributesToReplace)
    {
        var attributeType = attributesOwnerDeclaration.TryGetAnnotationAttributeType(AnnotationAttributeTypeName);
        if (attributeType is { } && CanBeAnnotated(attributesOwnerDeclaration.DeclaredElement, attributesOwnerDeclaration))
        {
            attributesToReplace = GetAttributesToReplace(attributesOwnerDeclaration);

            return factory => factory.CreateAttribute(attributeType, GetAnnotationArguments(), []);
        }

        attributesToReplace = [];
        return null;
    }

    [Pure]
    protected virtual IAttribute[] GetAttributesToReplace(IAttributesOwnerDeclaration ownerDeclaration) => [];

    [Pure]
    protected abstract bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context);
}