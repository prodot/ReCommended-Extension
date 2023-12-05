using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeAnnotations;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using ReCommendedExtension.ContextActions.CodeContracts.Internal;

namespace ReCommendedExtension.ContextActions.CodeContracts;

public abstract class AddContractContextAction : ContextActionBase
{
    ContractInfo? contractInfo;

    private protected AddContractContextAction(ICSharpContextActionDataProvider provider) => Provider = provider;

    void AddAnnotation()
    {
        if (TryGetAnnotationAttributeTypeName() is { } annotationAttributeTypeName)
        {
            var attributesOwnerDeclaration = Provider.GetSelectedElement<IAttributesOwnerDeclaration>(true, false);

            var attributeType = attributesOwnerDeclaration
                ?.GetPsiServices()
                .GetComponent<CodeAnnotationsConfiguration>()
                .GetAttributeTypeForElement(attributesOwnerDeclaration, annotationAttributeTypeName);

            if (attributeType is { })
            {
                Debug.Assert(attributesOwnerDeclaration is { });

                if (attributesOwnerDeclaration.Attributes.All(
                    attribute => attribute.GetAttributeType().GetClrName().ShortName != annotationAttributeTypeName))
                {
                    var factory = CSharpElementFactory.GetInstance(attributesOwnerDeclaration);

                    var attribute = factory.CreateAttribute(attributeType);

                    attributesOwnerDeclaration.AddAttributeAfter(attribute, attributesOwnerDeclaration.Attributes.LastOrDefault());
                }
            }
        }
    }

    protected ICSharpContextActionDataProvider Provider { get; }

    [Pure]
    protected virtual string? TryGetAnnotationAttributeTypeName() => null;

    [Pure]
    protected abstract bool IsAvailableForType(IType type);

    [Pure]
    protected abstract string GetContractTextForUI(string contractIdentifier);

    [Pure]
    protected abstract IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression);

    [MemberNotNullWhen(true, nameof(contractInfo))]
    public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache)
    {
        var declaration = Provider.GetSelectedElement<IDeclaration>(true, false);

        if (declaration.IsNullableAnnotationsContextEnabled())
        {
            return false;
        }

        contractInfo = ContractInfo.TryCreate(declaration, Provider.SelectedTreeRange, IsAvailableForType);

        return contractInfo is { };
    }

    public sealed override string Text
    {
        get
        {
            Debug.Assert(contractInfo is { });

            return $"Add contract ({contractInfo.GetContractKindForUI()}): {GetContractTextForUI(contractInfo.GetContractIdentifierForUI())}";
        }
    }

    protected sealed override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        Debug.Assert(contractInfo is { });

        AddAnnotation();

        contractInfo.AddContracts(
            Provider,
            expression => GetExpression(CSharpElementFactory.GetInstance(expression), expression),
            out var firstNonContractStatements);

        return textControl =>
        {
            if (firstNonContractStatements is { })
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