using JetBrains.Application.Progress;
using JetBrains.Application.UI.Controls.BulbMenu.Anchors;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.ContextActions;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Intentions.ContextActions.Inject;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Impl.Shared.InjectedPsi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Resources.Icons;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.UI.RichText;
using JetBrains.Util;

namespace ReCommendedExtension.ContextActions;

[ContextAction(
    Group = "C#",
    Name = "Set language injection for string literals" + ZoneMarker.Suffix,
    Description = "Set language injection for string literals.")]
public sealed class SetLanguageInjection(ICSharpContextActionDataProvider provider) : IContextAction
{
    sealed class InjectLanguageActionItem(IInjectorProviderInLiterals injectorProvider, ITreeNode ownerNode) : BulbActionBase
    {
        [Pure]
        ICSharpCommentNode CreateCommentNode(CSharpElementFactory factory, CommentType commentType)
            => commentType switch
            {
                CommentType.END_OF_LINE_COMMENT => factory.CreateComment($"// {InjectorProvider.LanguageEqualsCommentTexts[0]}"),
                CommentType.MULTILINE_COMMENT => factory.CreateComment($"/* {InjectorProvider.LanguageEqualsCommentTexts[0]} */"),

                _ => throw new NotSupportedException(),
            };

        public IInjectorProviderInLiterals InjectorProvider { get; } = injectorProvider;

        public HashSet<string>? LanguageEqualsCommentTexts { get; set; }

        public override string Text => InjectorProvider.InjectDescription;

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            Debug.Assert(LanguageEqualsCommentTexts is { });

            using (WriteLockCookie.Create())
            {
                var node = ownerNode;
                var commentType = CommentType.MULTILINE_COMMENT;

                switch (node.Parent)
                {
                    case IExpressionInitializer { Parent: IMultipleDeclarationMember { MultipleDeclaration.Declarators: [_] } declarationMember }:
                        node = declarationMember.MultipleDeclaration;
                        commentType = CommentType.END_OF_LINE_COMMENT;
                        break;

                    case IMultipleDeclarationMember { MultipleDeclaration.Declarators: [_] } declarationMember:
                        node = declarationMember.MultipleDeclaration;
                        commentType = CommentType.END_OF_LINE_COMMENT;
                        break;

                    case IAssignmentExpression assignmentExpression:
                        node = assignmentExpression;
                        commentType = CommentType.END_OF_LINE_COMMENT;
                        break;
                }

                var factory = CSharpElementFactory.GetInstance(node);

                var previousNonWhitespaceToken = node.PrevTokens().FirstOrDefault(t => t is not IWhitespaceNode);

                if (previousNonWhitespaceToken is ICSharpCommentNode
                    {
                        CommentType: CommentType.END_OF_LINE_COMMENT or CommentType.MULTILINE_COMMENT,
                    } currentCommentNode
                    && LanguageEqualsCommentTexts.Contains(currentCommentNode.CommentText.Trim()))
                {
                    currentCommentNode.ReplaceBy(CreateCommentNode(factory, currentCommentNode.CommentType));
                }
                else
                {
                    ModificationUtil.AddChildBefore(node, CreateCommentNode(factory, commentType));
                }
            }

            return _ => { };
        }
    }

    static readonly IAnchor submenuAnchor = new SubmenuAnchor(
        IntentionsAnchors.ContextActionsAnchor,
        new RichText("Set language injection or reference"));

    public bool IsAvailable(IUserDataHolder cache)
    {
        var injectorProviders = provider.Solution.GetComponent<InjectorProvidersInLiteralsViewer>().Providers;
        if (injectorProviders is not { })
        {
            return false;
        }

        var languageType = provider.PsiFile.Language;

        return injectorProviders.Any(
            injectorProvider =>
            {
                var node = injectorProvider.LocateInjectNodeByTreeOffset(
                    provider.PsiFile,
                    provider.Document,
                    provider.DocumentSelection.StartOffset.Offset);

                if (node is not { }
                    || !injectorProvider.IsSupportedLiteralForInjection(node)
                    || injectorProvider is ILanguageInjectorProviderInLiterals languageInjectorProviderInLiterals
                    && languageType.IsLanguage(languageInjectorProviderInLiterals.ProvidedLanguage))
                {
                    return false;
                }

                return languageType.IsLanguage(injectorProvider.SupportedOriginalLanguage);
            });
    }

    public IEnumerable<IntentionAction> CreateBulbItems()
    {
        var viewer = provider.Solution.GetComponent<InjectorProvidersInLiteralsViewer>();

        var languageType = provider.PsiFile.Language;

        var actionItems = (
            from injectorProvider in viewer.Providers
            where injectorProvider.SupportsInjectionComment
            let node =
                injectorProvider.LocateInjectNodeByTreeOffset(provider.PsiFile, provider.Document, provider.DocumentSelection.StartOffset.Offset)
            where node is { } && injectorProvider.IsSupportedLiteralForInjection(node)
            let languageInjectorProviderInLiterals = injectorProvider as ILanguageInjectorProviderInLiterals
            where (languageInjectorProviderInLiterals is not { } || !languageType.IsLanguage(languageInjectorProviderInLiterals.ProvidedLanguage))
                && languageType.IsLanguage(injectorProvider.SupportedOriginalLanguage)
            orderby injectorProvider.Priority
            select new InjectLanguageActionItem(injectorProvider, node)).ToList();

        var languageEqualsCommentTexts = new HashSet<string>(
            from actionItem in actionItems select actionItem.InjectorProvider.LanguageEqualsCommentTexts[0],
            StringComparer.OrdinalIgnoreCase);

        foreach (var actionItem in actionItems)
        {
            actionItem.LanguageEqualsCommentTexts = languageEqualsCommentTexts;

            yield return actionItem.ToContextActionIntention(
                submenuAnchor,
                actionItem.InjectorProvider.Icon ?? PsiFeaturesUnsortedThemedIcons.LangInjection.Id);
        }
    }
}