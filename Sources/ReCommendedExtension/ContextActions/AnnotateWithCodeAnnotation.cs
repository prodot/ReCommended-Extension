using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReCommendedExtension.ContextActions;

public abstract class AnnotateWithCodeAnnotation(ICSharpContextActionDataProvider provider) : AnnotateWith(provider)
{
    protected virtual AttributeValue[] AnnotationArguments => Array.Empty<AttributeValue>();

    protected sealed override bool IsAttribute(IAttribute attribute)
        => attribute.GetAttributeType().GetClrName().ShortName == AnnotationAttributeTypeName;

    protected sealed override Func<CSharpElementFactory, IAttribute>? CreateAttributeFactoryIfAvailable(
        IAttributesOwnerDeclaration attributesOwnerDeclaration,
        IPsiModule psiModule,
        out IAttribute? attributeToReplace)
    {
        var attributeType = attributesOwnerDeclaration.GetPsiServices()
            .GetComponent<CodeAnnotationsConfiguration>()
            .GetAttributeTypeForElement(attributesOwnerDeclaration, AnnotationAttributeTypeName);
        if (attributeType is { } && CanBeAnnotated(attributesOwnerDeclaration.DeclaredElement, attributesOwnerDeclaration))
        {
            attributeToReplace = TryGetAttributeToReplace(attributesOwnerDeclaration);

            return factory => factory.CreateAttribute(attributeType, AnnotationArguments, Array.Empty<Pair<string, AttributeValue>>());
        }

        attributeToReplace = null;
        return null;
    }

    protected virtual IAttribute? TryGetAttributeToReplace(IAttributesOwnerDeclaration ownerDeclaration) => null;

    protected abstract bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context);
}