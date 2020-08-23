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
using JetBrains.Util;

namespace ReCommendedExtension.ContextActions
{
    public abstract class AnnotateWithCodeAnnotation : AnnotateWith
    {
        protected AnnotateWithCodeAnnotation([NotNull] ICSharpContextActionDataProvider provider) : base(provider) { }

        [NotNull]
        protected virtual AttributeValue[] AnnotationArguments => Array.Empty<AttributeValue>();

        protected sealed override bool IsAttribute(IAttribute attribute)
            => attribute.GetAttributeInstance().GetAttributeType().GetClrName().ShortName == AnnotationAttributeTypeName;

        protected sealed override Func<CSharpElementFactory, IAttribute> CreateAttributeFactoryIfAvailable(
            IAttributesOwnerDeclaration attributesOwnerDeclaration,
            IPsiModule psiModule,
            out IAttribute attributeToReplace)
        {
            var attributeType = attributesOwnerDeclaration.GetPsiServices()
                .GetComponent<CodeAnnotationsConfiguration>()
                .GetAttributeTypeForElement(attributesOwnerDeclaration, AnnotationAttributeTypeName);
            if (attributeType != null && CanBeAnnotated(attributesOwnerDeclaration.DeclaredElement, attributesOwnerDeclaration, psiModule))
            {
                attributeToReplace = TryGetAttributeToReplace(attributesOwnerDeclaration);

                return factory =>
                {
                    Debug.Assert(factory != null);

                    return factory.CreateAttribute(attributeType, AnnotationArguments, Array.Empty<Pair<string, AttributeValue>>());
                };
            }

            attributeToReplace = null;
            return null;
        }

        [CanBeNull]
        protected virtual IAttribute TryGetAttributeToReplace([NotNull] IAttributesOwnerDeclaration ownerDeclaration) => null;

        protected abstract bool CanBeAnnotated(
            [CanBeNull] IDeclaredElement declaredElement,
            [NotNull] ITreeNode context,
            [NotNull] IPsiModule psiModule);
    }
}