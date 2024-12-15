﻿using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using ReCommendedExtension.ContextActions.CodeContracts.Internal;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.ContextActions.CodeContracts;

public abstract class AddContractContextAction(ICSharpContextActionDataProvider provider) : ContextAction<IDeclaration>(provider)
{
    ContractInfo? contractInfo;

    void AddAnnotation()
    {
        if (TryGetAnnotationAttributeTypeName() is { } annotationAttributeTypeName
            && Provider.GetSelectedElement<IAttributesOwnerDeclaration>(true, false) is { } attributesOwnerDeclaration)
        {
            var attributeType = attributesOwnerDeclaration.TryGetAnnotationAttributeType(annotationAttributeTypeName);
            if (attributeType is { } && attributesOwnerDeclaration.Attributes.All(
                attribute => attribute.GetAttributeType().GetClrName().ShortName != annotationAttributeTypeName))
            {
                var factory = CSharpElementFactory.GetInstance(attributesOwnerDeclaration);

                var attribute = factory.CreateAttribute(attributeType);

                attributesOwnerDeclaration.AddAttributeAfter(attribute, attributesOwnerDeclaration.Attributes.LastOrDefault());
            }
        }
    }

    protected ICSharpContextActionDataProvider Provider { get; } = provider;

    [Pure]
    protected virtual string? TryGetAnnotationAttributeTypeName() => null;

    [Pure]
    protected abstract bool IsAvailableForType(IType type);

    [Pure]
    protected abstract string GetContractTextForUI(string contractIdentifier);

    [Pure]
    protected abstract IExpression GetExpression(CSharpElementFactory factory, IExpression contractExpression);

    [MemberNotNullWhen(true, nameof(contractInfo))]
    protected sealed override bool IsAvailable(IDeclaration selectedElement)
    {
        if (selectedElement.IsNullableAnnotationsContextEnabled())
        {
            return false;
        }

        contractInfo = ContractInfo.TryCreate(selectedElement, Provider.SelectedTreeRange, IsAvailableForType);

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

                    var coordinates = textControl.Document.GetCoordsByOffset(firstNonContractStatement.GetDocumentRange().TextRange.StartDocOffset());
                    textControl.Caret.MoveTo(coordinates, CaretVisualPlacement.DontScrollIfVisible);

                    textControl.EmulateEnter();

                    textControl.Caret.MoveTo(originalPosition, CaretVisualPlacement.DontScrollIfVisible);
                }
            }
        };
    }
}