using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Impl;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using ReCommendedExtension.ContextActions.CodeContracts.Internal;

namespace ReCommendedExtension.ContextActions.CodeContracts
{
    public abstract class AddContractContextAction : ContextActionBase
    {
        ContractInfo contractInfo;

        internal AddContractContextAction([NotNull] ICSharpContextActionDataProvider provider)
        {
            Provider = provider;
        }

        void AddAnnotation()
        {
            var annotationAttributeTypeName = TryGetAnnotationAttributeTypeName();
            if (annotationAttributeTypeName != null)
            {
                var attributesOwnerDeclaration = Provider.GetSelectedElement<IAttributesOwnerDeclaration>(true, false);

                var codeAnnotationsConfiguration = attributesOwnerDeclaration?.GetPsiServices().GetComponent<CodeAnnotationsConfiguration>();

                var attributeType = codeAnnotationsConfiguration?.GetAttributeTypeForElement(attributesOwnerDeclaration, annotationAttributeTypeName);

                if (attributeType != null &&
                    attributesOwnerDeclaration.AttributesEnumerable.All(
                        attribute =>
                            attribute.AssertNotNull().GetAttributeInstance().GetAttributeType().GetClrName().ShortName != annotationAttributeTypeName))
                {
                    var factory = CSharpElementFactory.GetInstance(Provider.PsiModule);

                    var attribute = factory.CreateAttribute(attributeType);

                    attributesOwnerDeclaration.AddAttributeAfter(attribute, attributesOwnerDeclaration.AttributesEnumerable.LastOrDefault());
                }
            }
        }

        [NotNull]
        protected ICSharpContextActionDataProvider Provider { get; }

        protected virtual string TryGetAnnotationAttributeTypeName() => null;

        protected abstract bool IsAvailableForType([NotNull] IType type);

        [NotNull]
        protected abstract string GetContractTextForUI([NotNull] string contractIdentifier);

        [NotNull]
        protected abstract IExpression GetExpression([NotNull] CSharpElementFactory factory, [NotNull] IExpression contractExpression);

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache)
        {
            var declaration = Provider.GetSelectedElement<IDeclaration>(true, false);

            contractInfo = ContractInfo.TryCreate(declaration, Provider.SelectedTreeRange, IsAvailableForType);

            return contractInfo != null;
        }

        public sealed override string Text
        {
            get
            {
                Debug.Assert(contractInfo != null);

                return string.Format(
                    "Add contract ({0}): {1}",
                    contractInfo.GetContractKindForUI(),
                    GetContractTextForUI(contractInfo.GetContractIdentifierForUI()));
            }
        }

        protected sealed override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            AddAnnotation();

            Debug.Assert(contractInfo != null);

            ICollection<ICSharpStatement> firstNonContractStatements;
            contractInfo.AddContracts(
                Provider,
                expression => GetExpression(CSharpElementFactory.GetInstance(Provider.PsiModule), expression.AssertNotNull()),
                out firstNonContractStatements);

            return textControl =>
            {
                Debug.Assert(textControl != null);

                if (firstNonContractStatements != null)
                {
                    foreach (var firstNonContractStatement in firstNonContractStatements)
                    {
                        var originalPosition = textControl.Caret.PositionValue;

                        var coordinates = textControl.Document.GetCoordsByOffset(firstNonContractStatement.GetDocumentRange().TextRange.StartOffset);
                        textControl.Caret.MoveTo(coordinates, CaretVisualPlacement.DontScrollIfVisible);

                        textControl.EmulateEnter();

                        textControl.Caret.MoveTo(originalPosition, CaretVisualPlacement.DontScrollIfVisible);
                    }
                }
            };
        }
    }
}