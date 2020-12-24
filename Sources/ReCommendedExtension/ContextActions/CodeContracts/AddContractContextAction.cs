using System;
using System.Diagnostics;
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
        [CanBeNull]
        ContractInfo contractInfo;

        private protected AddContractContextAction([NotNull] ICSharpContextActionDataProvider provider) => Provider = provider;

        void AddAnnotation()
        {
            var annotationAttributeTypeName = TryGetAnnotationAttributeTypeName();
            if (annotationAttributeTypeName != null)
            {
                var attributesOwnerDeclaration = Provider.GetSelectedElement<IAttributesOwnerDeclaration>(true, false);

                var codeAnnotationsConfiguration = attributesOwnerDeclaration?.GetPsiServices().GetComponent<CodeAnnotationsConfiguration>();

                var attributeType = codeAnnotationsConfiguration?.GetAttributeTypeForElement(attributesOwnerDeclaration, annotationAttributeTypeName);

                if (attributeType != null &&
                    attributesOwnerDeclaration.Attributes.All(
                        attribute => attribute.AssertNotNull().GetAttributeInstance().GetAttributeType().GetClrName().ShortName !=
                            annotationAttributeTypeName))
                {
                    var factory = CSharpElementFactory.GetInstance(attributesOwnerDeclaration);

                    var attribute = factory.CreateAttribute(attributeType);

                    attributesOwnerDeclaration.AddAttributeAfter(attribute, attributesOwnerDeclaration.Attributes.LastOrDefault());
                }
            }
        }

        [NotNull]
        protected ICSharpContextActionDataProvider Provider { get; }

        [CanBeNull]
        protected virtual string TryGetAnnotationAttributeTypeName() => null;

        protected abstract bool IsAvailableForType([NotNull] IType type);

        [NotNull]
        protected abstract string GetContractTextForUI([NotNull] string contractIdentifier);

        [NotNull]
        protected abstract IExpression GetExpression([NotNull] CSharpElementFactory factory, [NotNull] IExpression contractExpression);

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache)
        {
            var declaration = Provider.GetSelectedElement<IDeclaration>(true, false);

            if (declaration.IsNullableAnnotationsContextEnabled())
            {
                return false;
            }

            contractInfo = ContractInfo.TryCreate(declaration, Provider.SelectedTreeRange, IsAvailableForType);

            return contractInfo != null;
        }

        public sealed override string Text
        {
            get
            {
                Debug.Assert(contractInfo != null);

                return $"Add contract ({contractInfo.GetContractKindForUI()}): {GetContractTextForUI(contractInfo.GetContractIdentifierForUI())}";
            }
        }

        protected sealed override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            AddAnnotation();

            Debug.Assert(contractInfo != null);

            contractInfo.AddContracts(
                Provider,
                expression => GetExpression(CSharpElementFactory.GetInstance(expression.AssertNotNull()), expression.AssertNotNull()),
                out var firstNonContractStatements);

            return textControl =>
            {
                Debug.Assert(textControl != null);

                if (firstNonContractStatements != null)
                {
                    foreach (var firstNonContractStatement in firstNonContractStatements)
                    {
                        var originalPosition = textControl.Caret.Position.Value;

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