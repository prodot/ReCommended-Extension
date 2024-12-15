using JetBrains.Metadata.Reader.Impl;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Impl.Special;

namespace ReCommendedExtension.ContextActions.Annotations;

public abstract class AnnotateWith<A>(ICSharpContextActionDataProvider provider) : AnnotateWith(provider) where A : Attribute
{
    protected sealed override string AnnotationAttributeTypeName => typeof(A).Name;

    protected sealed override bool IsAttribute(IAttribute attribute) => attribute.GetAttributeType().GetClrName().FullName == typeof(A).FullName;

    protected sealed override Func<CSharpElementFactory, IAttribute>? CreateAttributeFactoryIfAvailable(
        IAttributesOwnerDeclaration attributesOwnerDeclaration,
        out IAttribute[] attributesToReplace)
    {
        attributesToReplace = [];

        if (CanBeAnnotated(attributesOwnerDeclaration.DeclaredElement))
        {
            return factory =>
            {
                var fullName = typeof(A).FullName;
                Debug.Assert(fullName is { });

                return factory.CreateAttribute(new SpecialAttributeInstance(new ClrTypeName(fullName), PsiModule, GetAnnotationArguments));
            };
        }

        return null;
    }

    [Pure]
    protected abstract bool CanBeAnnotated(IDeclaredElement? declaredElement);
}