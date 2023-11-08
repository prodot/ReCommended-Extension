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

public abstract class AnnotateWith : ContextActionBase
{
    readonly ICSharpContextActionDataProvider provider;

    IAttributesOwnerDeclaration? attributesOwnerDeclaration;

    Func<CSharpElementFactory, IAttribute>? createAttributeFactory;

    IAttribute? attributeToReplace;

    protected AnnotateWith(ICSharpContextActionDataProvider provider) => this.provider = provider;

    protected abstract string AnnotationAttributeTypeName { get; }

    protected abstract bool IsAttribute(IAttribute attribute);

    protected abstract Func<CSharpElementFactory, IAttribute>? CreateAttributeFactoryIfAvailable(
        IAttributesOwnerDeclaration attributesOwnerDeclaration,
        IPsiModule psiModule,
        out IAttribute? attributeToReplace);

    protected virtual string TextSuffix => "";

    protected virtual bool AllowsMultiple => false;

    public sealed override string Text
    {
        get
        {
            var typeName = AnnotationAttributeTypeName;
            var textSuffix = TextSuffix;

            var attributeName = typeName.EndsWith(nameof(Attribute), StringComparison.Ordinal)
                ? typeName[..^nameof(Attribute).Length]
                : typeName;

            return $"Annotate with [{attributeName}]{(textSuffix != "" ? $" ({textSuffix})" : "")}";
        }
    }

    [MemberNotNullWhen(true, nameof(createAttributeFactory))]
    [MemberNotNullWhen(true, nameof(attributesOwnerDeclaration))]
    public sealed override bool IsAvailable(JetBrains.Util.IUserDataHolder cache)
    {
        attributesOwnerDeclaration = provider.GetSelectedElement<IAttributesOwnerDeclaration>(true, false);

        if (attributesOwnerDeclaration is { }
            && attributesOwnerDeclaration.GetNameRange().Contains(provider.SelectedTreeRange)
            && !attributesOwnerDeclaration.OverridesInheritedMember()
            && !attributesOwnerDeclaration.IsOnLocalFunctionWithUnsupportedAttributes()
            && !attributesOwnerDeclaration.IsOnLambdaExpressionWithUnsupportedAttributes()
            && !attributesOwnerDeclaration.IsOnAnonymousMethodWithUnsupportedAttributes()
            && (AllowsMultiple || !attributesOwnerDeclaration.Attributes.Any(IsAttribute)))
        {
            createAttributeFactory = CreateAttributeFactoryIfAvailable(attributesOwnerDeclaration, provider.PsiModule, out attributeToReplace);

            if (createAttributeFactory is { })
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
        try
        {
            Debug.Assert(attributesOwnerDeclaration is { });
            Debug.Assert(createAttributeFactory is { });

            IAttribute attribute;

            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(attributesOwnerDeclaration);

                attribute = createAttributeFactory(factory);

                attribute = attributeToReplace is { }
                    ? attributesOwnerDeclaration.ReplaceAttribute(attributeToReplace, attribute)
                    : attributesOwnerDeclaration.AddAttributeBefore(attribute, null); // add as last attribute

                if (attributesOwnerDeclaration is IParameter { ContainingParametersOwner: ILambdaExpression { ParameterDeclarations: [_] } }
                    and TreeElement { PrevSibling: not { }, NextSibling: not { } } treeElement)
                {
                    // parenthesize the parameter
                    ModificationUtil.AddChildBefore(treeElement, CSharpTokenType.LPARENTH.CreateLeafElement());
                    ModificationUtil.AddChildAfter(treeElement, CSharpTokenType.RPARENTH.CreateLeafElement());
                }

                ContextActionUtils.FormatWithDefaultProfile(attribute);
            }

            return textControl => ExecutePsiTransactionPostProcess(textControl, attribute);
        }
        finally
        {
            attributeToReplace = null;
            createAttributeFactory = null;
            attributesOwnerDeclaration = null;
        }
    }

    protected virtual void ExecutePsiTransactionPostProcess(ITextControl textControl, IAttribute attribute) { }
}