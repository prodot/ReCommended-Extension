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
    protected override bool IsAttribute(IAttribute attribute) => attribute.GetAttributeType().GetClrName().ShortName == AnnotationAttributeTypeName;

    protected sealed override Func<CSharpElementFactory, IAttribute>? CreateAttributeFactoryIfAvailable(
        IAttributesOwnerDeclaration attributesOwnerDeclaration,
        IPsiModule psiModule,
        out IAttribute[] attributesToReplace)
    {
        var attributeType = attributesOwnerDeclaration
            .GetPsiServices()
            .GetComponent<CodeAnnotationsConfiguration>()
            .GetAttributeTypeForElement(attributesOwnerDeclaration, AnnotationAttributeTypeName);
        if (attributeType is { } && CanBeAnnotated(attributesOwnerDeclaration.DeclaredElement, attributesOwnerDeclaration))
        {
            attributesToReplace = GetAttributesToReplace(attributesOwnerDeclaration);

            return factory => factory.CreateAttribute(attributeType, GetAnnotationArguments(psiModule), Array.Empty<Pair<string, AttributeValue>>());
        }

        attributesToReplace = Array.Empty<IAttribute>();
        return null;
    }

    [Pure]
    protected virtual IAttribute[] GetAttributesToReplace(IAttributesOwnerDeclaration ownerDeclaration) => Array.Empty<IAttribute>();

    [Pure]
    protected abstract bool CanBeAnnotated(IDeclaredElement? declaredElement, ITreeNode context);
}