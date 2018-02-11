using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReCommendedExtension.ContextActions
{
    public abstract class AnnotateWith : ContextActionBase
    {
        [NotNull]
        readonly ICSharpContextActionDataProvider provider;

        IAttributesOwnerDeclaration attributesOwnerDeclaration;

        Func<CSharpElementFactory, IAttribute> createAttributeFactory;
        IAttribute attributeToRemove;

        protected AnnotateWith([NotNull] ICSharpContextActionDataProvider provider) => this.provider = provider;

        [NotNull]
        protected abstract string AnnotationAttributeTypeName { get; }

        protected abstract bool IsAttribute([NotNull] IAttribute attribute);

        protected abstract Func<CSharpElementFactory, IAttribute> CreateAttributeFactoryIfAvailable(
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration,
            [NotNull] IPsiModule psiModule,
            out IAttribute attributeToRemove);

        [NotNull]
        protected virtual string TextSuffix => "";

        public sealed override string Text
        {
            get
            {
                var typeName = AnnotationAttributeTypeName;
                var textSuffix = TextSuffix;

                return string.Format(
                    "Annotate with [{0}]{1}",
                    typeName.EndsWith(nameof(Attribute), StringComparison.Ordinal)
                        ? typeName.Substring(0, typeName.Length - nameof(Attribute).Length)
                        : typeName,
                    textSuffix != "" ? string.Format(" ({0})", textSuffix) : "");
            }
        }

        public sealed override bool IsAvailable(JetBrains.Util.IUserDataHolder cache)
        {
            attributesOwnerDeclaration = provider.GetSelectedElement<IAttributesOwnerDeclaration>(true, false);

            if (attributesOwnerDeclaration != null && attributesOwnerDeclaration.GetNameRange().Contains(provider.SelectedTreeRange) &&
                !attributesOwnerDeclaration.OverridesInheritedMember() && !attributesOwnerDeclaration.AttributesEnumerable.Any(IsAttribute))
            {
                createAttributeFactory = CreateAttributeFactoryIfAvailable(attributesOwnerDeclaration, provider.PsiModule, out attributeToRemove);

                if (createAttributeFactory != null)
                {
                    return true;
                }
            }

            attributeToRemove = null;
            createAttributeFactory = null;
            attributesOwnerDeclaration = null;

            return false;
        }

        protected sealed override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            Debug.Assert(attributesOwnerDeclaration != null);
            Debug.Assert(createAttributeFactory != null);

            try
            {
                using (WriteLockCookie.Create())
                {
                    var factory = CSharpElementFactory.GetInstance(attributesOwnerDeclaration);

                    var attribute = createAttributeFactory(factory);

                    Debug.Assert(attributesOwnerDeclaration != null);
                    Debug.Assert(attribute != null);

                    attributesOwnerDeclaration.AddAttributeAfter(attribute, attributeToRemove);
                    if (attributeToRemove != null)
                    {
                        attributesOwnerDeclaration.RemoveAttribute(attributeToRemove);
                    }

                    ContextActionUtils.FormatWithDefaultProfile(attribute);
                }

                return _ => { };
            }
            finally
            {
                attributeToRemove = null;
                createAttributeFactory = null;
                attributesOwnerDeclaration = null;
            }
        }
    }
}