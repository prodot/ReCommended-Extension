using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.ContextActions.Annotations;

public abstract class AnnotateWith(ICSharpContextActionDataProvider provider) : ContextAction<IAttributesOwnerDeclaration>(provider)
{
    IAttributesOwnerDeclaration? attributesOwnerDeclaration;

    Func<CSharpElementFactory, IAttribute>? createAttributeFactory;

    IAttribute[] attributesToReplace = [];

    protected abstract string AnnotationAttributeTypeName { get; }

    [Pure]
    protected abstract bool IsAttribute(IAttribute attribute);

    [Pure]
    protected abstract Func<CSharpElementFactory, IAttribute>? CreateAttributeFactoryIfAvailable(
        IAttributesOwnerDeclaration attributesOwnerDeclaration,
        out IAttribute[] attributeToReplace);

    protected virtual string TextSuffix => "";

    protected virtual bool AllowsInheritedMethods => false;

    protected virtual bool AllowsMultiple => false;

    protected virtual bool AnnotateMethodReturnValue => false;

    [Pure]
    protected virtual AttributeValue[] GetAnnotationArguments() => [];

    [MemberNotNullWhen(true, nameof(createAttributeFactory))]
    [MemberNotNullWhen(true, nameof(attributesOwnerDeclaration))]
    protected sealed override bool IsAvailable(IAttributesOwnerDeclaration selectedElement)
    {
        if (selectedElement.GetNameRange().Contains(SelectedTreeRange)
            && (AllowsInheritedMethods || !selectedElement.OverridesInheritedMember())
            && !selectedElement.IsOnLocalFunctionWithUnsupportedAttributes()
            && !selectedElement.IsOnLambdaExpressionWithUnsupportedAttributes()
            && !selectedElement.IsOnAnonymousMethodWithUnsupportedAttributes()
            && (AllowsMultiple || !selectedElement.Attributes.Any(IsAttribute)))
        {
            createAttributeFactory = CreateAttributeFactoryIfAvailable(selectedElement, out attributesToReplace);

            if (createAttributeFactory is { })
            {
                attributesOwnerDeclaration = selectedElement;
                return true;
            }
        }

        attributesToReplace = [];
        createAttributeFactory = null;
        attributesOwnerDeclaration = null;

        return false;
    }

    public sealed override string Text
    {
        get
        {
            var typeName = AnnotationAttributeTypeName;

            var attributeName = typeName.EndsWith(nameof(Attribute), StringComparison.Ordinal)
                ? typeName[..^nameof(Attribute).Length]
                : typeName;

            var annotationArguments = GetAnnotationArguments();

            var arguments = annotationArguments is [] or [{ ConstantValue.Kind: ConstantValueKind.NonCompileTimeConstant }]
                ? ""
                : $"({
                    string.Join(
                        ", ",
                        from a in annotationArguments
                        select a.ConstantValue.GetPresentation(CSharpLanguage.Instance, TypePresentationStyle.Default).Text)
                })";

            var textSuffix = TextSuffix != "" ? $" ({TextSuffix})" : "";

            return $"Annotate with [{attributeName}{arguments}]{textSuffix}";
        }
    }

    protected sealed override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        try
        {
            Debug.Assert(attributesOwnerDeclaration is { });
            Debug.Assert(createAttributeFactory is { });

            IAttribute attribute;

            using (WriteLockCookie.Create())
            {
                AttributeTarget attributeTarget;

                switch (attributesOwnerDeclaration)
                {
                    case IPrimaryConstructorDeclaration primaryConstructorDeclaration:
                        attributesOwnerDeclaration = primaryConstructorDeclaration.GetContainingTypeDeclaration();
                        Debug.Assert(attributesOwnerDeclaration is { });

                        attributeTarget = AttributeTarget.Method;
                        break;

                    case IMethodDeclaration when AnnotateMethodReturnValue:
                        attributeTarget = AttributeTarget.Return;
                        break;

                    default:
                        attributeTarget = AttributeTarget.None;
                        break;
                }

                var factory = CSharpElementFactory.GetInstance(attributesOwnerDeclaration);

                attribute = createAttributeFactory(factory);
                attribute.SetTarget(attributeTarget);

                if (attributesToReplace is [])
                {
                    // add as a last attribute
                    attribute = attributesOwnerDeclaration.AddAttributeBefore(attribute, null);
                }
                else
                {
                    // replace the last one
                    attribute = attributesOwnerDeclaration.ReplaceAttribute(attributesToReplace[^1], attribute);

                    // remove all attributes except the last one
                    for (var i = attributesToReplace.Length - 2; i >= 0; i--)
                    {
                        attributesOwnerDeclaration.RemoveAttribute(attributesToReplace[i]);
                    }
                }

                if (attributesOwnerDeclaration is IParameter { ContainingParametersOwner: ILambdaExpression { ParameterDeclarations: [_] } }
                    and TreeElement { PrevSibling: null, NextSibling: null } treeElement)
                {
                    // parenthesize the parameter
                    ModificationUtil.AddChildBefore(treeElement, CSharpTokenType.LPARENTH.CreateLeafElement());
                    ModificationUtil.AddChildAfter(treeElement, CSharpTokenType.RPARENTH.CreateLeafElement());
                }

                ContextActionUtils.FormatWithDefaultProfile(attribute);
            }

            return textControl =>
            {
                if (GetAnnotationArguments() is [{ ConstantValue.Kind: ConstantValueKind.NonCompileTimeConstant }])
                {
                    textControl.Caret.MoveTo(attribute.Arguments[0].GetDocumentRange().EndOffset, CaretVisualPlacement.DontScrollIfVisible);
                    textControl.EmulateAction("TextControl.Backspace");
                }
            };
        }
        finally
        {
            attributesToReplace = [];
            createAttributeFactory = null;
            attributesOwnerDeclaration = null;
        }
    }
}