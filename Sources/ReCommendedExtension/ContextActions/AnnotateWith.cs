using System;
using System.Diagnostics;
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

        [CanBeNull]
        IAttributesOwnerDeclaration attributesOwnerDeclaration;

        [CanBeNull]
        Func<CSharpElementFactory, IAttribute> createAttributeFactory;
        [CanBeNull]
        IAttribute attributeToReplace;

        protected AnnotateWith([NotNull] ICSharpContextActionDataProvider provider) => this.provider = provider;

        [NotNull]
        protected abstract string AnnotationAttributeTypeName { get; }

        protected abstract bool IsAttribute([NotNull] IAttribute attribute);

        [CanBeNull]
        protected abstract Func<CSharpElementFactory, IAttribute> CreateAttributeFactoryIfAvailable(
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration,
            [NotNull] IPsiModule psiModule,
            [CanBeNull] out IAttribute attributeToReplace);

        [NotNull]
        protected virtual string TextSuffix => "";

        protected virtual bool AllowsMultiple => false;

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

            if (attributesOwnerDeclaration != null &&
                attributesOwnerDeclaration.GetNameRange().Contains(provider.SelectedTreeRange) &&
                !attributesOwnerDeclaration.OverridesInheritedMember() &&
                !attributesOwnerDeclaration.IsOnLocalFunctionWithUnsupportedAttributes() &&
                (AllowsMultiple || !attributesOwnerDeclaration.Attributes.Any(IsAttribute)))
            {
                Debug.Assert(attributesOwnerDeclaration != null);

                createAttributeFactory = CreateAttributeFactoryIfAvailable(attributesOwnerDeclaration, provider.PsiModule, out attributeToReplace);

                if (createAttributeFactory != null)
                {
                    return true;
                }
            }

            attributeToReplace = null;
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
                IAttribute attribute;

                using (WriteLockCookie.Create())
                {
                    var factory = CSharpElementFactory.GetInstance(attributesOwnerDeclaration);

                    attribute = createAttributeFactory(factory);

                    Debug.Assert(attribute != null);

                    attribute = attributeToReplace != null
                        ? attributesOwnerDeclaration.ReplaceAttribute(attributeToReplace, attribute)
                        : attributesOwnerDeclaration.AddAttributeBefore(attribute, null); // add as last attribute

                    ContextActionUtils.FormatWithDefaultProfile(attribute);
                }

                return textControl =>
                {
                    Debug.Assert(textControl != null);
                    ExecutePsiTransactionPostProcess(textControl, attribute);
                };
            }
            finally
            {
                attributeToReplace = null;
                createAttributeFactory = null;
                attributesOwnerDeclaration = null;
            }
        }

        protected virtual void ExecutePsiTransactionPostProcess([NotNull] ITextControl textControl, [NotNull] IAttribute attribute) { }
    }
}