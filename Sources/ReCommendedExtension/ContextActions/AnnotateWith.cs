using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Intentions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Modules;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReCommendedExtension.ContextActions;

public abstract class AnnotateWith(ICSharpContextActionDataProvider provider) : ContextActionBase
{
    IAttributesOwnerDeclaration? attributesOwnerDeclaration;

    Func<CSharpElementFactory, IAttribute>? createAttributeFactory;

    IAttribute[] attributesToReplace = Array.Empty<IAttribute>();

    protected abstract string AnnotationAttributeTypeName { get; }

    [Pure]
    protected abstract bool IsAttribute(IAttribute attribute);

    [Pure]
    protected abstract Func<CSharpElementFactory, IAttribute>? CreateAttributeFactoryIfAvailable(
        IAttributesOwnerDeclaration attributesOwnerDeclaration,
        IPsiModule psiModule,
        out IAttribute[] attributeToReplace);

    protected virtual string TextSuffix => "";

    protected virtual bool AllowsInheritedMethods => false;

    protected virtual bool AllowsMultiple => false;

    [Pure]
    protected virtual AttributeValue[] GetAnnotationArguments(IPsiModule psiModule) => Array.Empty<AttributeValue>();

    [MemberNotNullWhen(true, nameof(createAttributeFactory))]
    [MemberNotNullWhen(true, nameof(attributesOwnerDeclaration))]
    public sealed override bool IsAvailable(JetBrains.Util.IUserDataHolder cache)
    {
        attributesOwnerDeclaration = provider.GetSelectedElement<IAttributesOwnerDeclaration>(true, false);

        if (attributesOwnerDeclaration is { }
            && attributesOwnerDeclaration.GetNameRange().Contains(provider.SelectedTreeRange)
            && (AllowsInheritedMethods || !attributesOwnerDeclaration.OverridesInheritedMember())
            && !attributesOwnerDeclaration.IsOnLocalFunctionWithUnsupportedAttributes()
            && !attributesOwnerDeclaration.IsOnLambdaExpressionWithUnsupportedAttributes()
            && !attributesOwnerDeclaration.IsOnAnonymousMethodWithUnsupportedAttributes()
            && (AllowsMultiple || !attributesOwnerDeclaration.Attributes.Any(IsAttribute)))
        {
            createAttributeFactory = CreateAttributeFactoryIfAvailable(attributesOwnerDeclaration, provider.PsiModule, out attributesToReplace);

            if (createAttributeFactory is { })
            {
                return true;
            }
        }

        attributesToReplace = Array.Empty<IAttribute>();
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

            var annotationArguments = GetAnnotationArguments(provider.PsiModule);

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
                var attributeTarget = AttributeTarget.None;

                if (attributesOwnerDeclaration is IPrimaryConstructorDeclaration primaryConstructorDeclaration)
                {
                    attributesOwnerDeclaration = primaryConstructorDeclaration.GetContainingTypeDeclaration();
                    Debug.Assert(attributesOwnerDeclaration is { });

                    attributeTarget = AttributeTarget.Method;
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
                    and TreeElement { PrevSibling: not { }, NextSibling: not { } } treeElement)
                {
                    // parenthesize the parameter
                    ModificationUtil.AddChildBefore(treeElement, CSharpTokenType.LPARENTH.CreateLeafElement());
                    ModificationUtil.AddChildAfter(treeElement, CSharpTokenType.RPARENTH.CreateLeafElement());
                }

                ContextActionUtils.FormatWithDefaultProfile(attribute);
            }

            return textControl =>
            {
                if (GetAnnotationArguments(provider.PsiModule) is [{ ConstantValue.Kind: ConstantValueKind.NonCompileTimeConstant }])
                {
                    textControl.Caret.MoveTo(attribute.Arguments[0].GetDocumentRange().EndOffset, CaretVisualPlacement.DontScrollIfVisible);
                    textControl.EmulateAction("TextControl.Backspace");
                }
            };
        }
        finally
        {
            attributesToReplace = Array.Empty<IAttribute>();
            createAttributeFactory = null;
            attributesOwnerDeclaration = null;
        }
    }
}