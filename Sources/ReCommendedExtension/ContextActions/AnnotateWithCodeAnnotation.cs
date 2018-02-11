using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.ContextActions
{
    public abstract class AnnotateWithCodeAnnotation : AnnotateWith
    {
        protected AnnotateWithCodeAnnotation([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        protected sealed override bool IsAttribute(IAttribute attribute)
            => attribute.GetAttributeInstance().GetAttributeType().GetClrName().ShortName == AnnotationAttributeTypeName;

        protected sealed override Func<CSharpElementFactory, IAttribute> CreateAttributeFactoryIfAvailable(
            IAttributesOwnerDeclaration attributesOwnerDeclaration,
            IPsiModule psiModule,
            out IAttribute attributeToRemove)
        {
            var attributeType =
                attributesOwnerDeclaration.GetPsiServices()
                    .GetComponent<CodeAnnotationsConfiguration>()
                    .GetAttributeTypeForElement(attributesOwnerDeclaration, AnnotationAttributeTypeName);
            if (attributeType != null && CanBeAnnotated(attributesOwnerDeclaration.DeclaredElement, attributesOwnerDeclaration, psiModule))
            {
                attributeToRemove = TryGetAttributeToReplace(attributesOwnerDeclaration);

                return factory =>
                {
                    Debug.Assert(factory != null);

                    return factory.CreateAttribute(attributeType);
                };
            }

            attributeToRemove = null;
            return null;
        }

        protected virtual IAttribute TryGetAttributeToReplace([NotNull] IAttributesOwnerDeclaration ownerDeclaration) => null;

        protected abstract bool CanBeAnnotated(IDeclaredElement declaredElement, [NotNull] ITreeNode context, [NotNull] IPsiModule psiModule);
    }
}