using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.UI.Controls.BulbMenu.Anchors;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
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

namespace ReCommendedExtension.ContextActions
{
    [ContextAction(
        Group = "C#",
        Name = "Set language injection for string literals" + ZoneMarker.Suffix,
        Description = "Set language injection for string literals.")]
    public sealed class SetLanguageInjection : IContextAction
    {
        sealed class InjectLanguageActionItem : BulbActionBase
        {
            [NotNull]
            readonly ITreeNode injectorOwnerNode;

            public InjectLanguageActionItem([NotNull] IInjectorProviderInLiterals injectorProvider, [NotNull] ITreeNode injectorOwnerNode)
            {
                InjectorProvider = injectorProvider;
                this.injectorOwnerNode = injectorOwnerNode;
            }

            [NotNull]
            ICSharpCommentNode CreateCommentNode([NotNull] CSharpElementFactory factory, CommentType commentType)
            {
                Debug.Assert(InjectorProvider.LanguageEqualsCommentTexts != null);

                switch (commentType)
                {
                    case CommentType.END_OF_LINE_COMMENT: return factory.CreateComment("// " + InjectorProvider.LanguageEqualsCommentTexts[0]);

                    case CommentType.MULTILINE_COMMENT: return factory.CreateComment("/* " + InjectorProvider.LanguageEqualsCommentTexts[0] + " */");

                    default: throw new NotSupportedException();
                }
            }

            [NotNull]
            public IInjectorProviderInLiterals InjectorProvider { get; }

            [CanBeNull]
            [ItemNotNull]
            public HashSet<string> LanguageEqualsCommentTexts { get; set; }

            public override string Text => InjectorProvider.InjectDescription.AssertNotNull();

            protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
            {
                using (WriteLockCookie.Create())
                {
                    var node = injectorOwnerNode;
                    var commentType = CommentType.MULTILINE_COMMENT;

                    switch (node.Parent)
                    {
                        case IExpressionInitializer expressionInitializer
                            when expressionInitializer.Parent is IMultipleDeclarationMember declarationMember &&
                            declarationMember.MultipleDeclaration?.Declarators.Count == 1:
                            node = declarationMember.MultipleDeclaration;
                            commentType = CommentType.END_OF_LINE_COMMENT;
                            break;

                        case IMultipleDeclarationMember declarationMember when declarationMember.MultipleDeclaration?.Declarators.Count == 1:
                            node = declarationMember.MultipleDeclaration;
                            commentType = CommentType.END_OF_LINE_COMMENT;
                            break;

                        case IAssignmentExpression assignmentExpression:
                            node = assignmentExpression;
                            commentType = CommentType.END_OF_LINE_COMMENT;
                            break;
                    }

                    Debug.Assert(LanguageEqualsCommentTexts != null);

                    var factory = CSharpElementFactory.GetInstance(node);

                    var previousNonWhitespaceToken = node.PrevTokens().FirstOrDefault(t => !(t is IWhitespaceNode));

                    if (previousNonWhitespaceToken is ICSharpCommentNode currentCommentNode &&
                        (currentCommentNode.CommentType == CommentType.END_OF_LINE_COMMENT ||
                            currentCommentNode.CommentType == CommentType.MULTILINE_COMMENT) &&
                        LanguageEqualsCommentTexts.Contains(currentCommentNode.CommentText.Trim()))
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

        [NotNull]
        static readonly IAnchor submenuAnchor = new SubmenuAnchor(
            IntentionsAnchors.ContextActionsAnchor,
            new RichText("Set language injection or reference"));

        [NotNull]
        readonly ICSharpContextActionDataProvider provider;

        public SetLanguageInjection([NotNull] ICSharpContextActionDataProvider provider) => this.provider = provider;

        public bool IsAvailable(IUserDataHolder cache)
        {
            var injectorProviders = provider.Solution.GetComponent<InjectorProvidersInLiteralsViewer>().Providers;
            if (injectorProviders == null)
            {
                return false;
            }

            var languageType = provider.PsiFile.Language;

            return injectorProviders.Any(
                injectorProvider =>
                {
                    Debug.Assert(injectorProvider != null);

                    var node = injectorProvider.LocateInjectNodeByTreeOffset(
                        provider.PsiFile,
                        provider.Document,
                        provider.DocumentSelection.StartOffset.Offset);

                    if (node == null ||
                        !injectorProvider.IsSupportedLiteralForInjection(node) ||
                        injectorProvider is ILanguageInjectorProviderInLiterals languageInjectorProviderInLiterals &&
                        languageType.IsLanguage(languageInjectorProviderInLiterals.ProvidedLanguage))
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

            Debug.Assert(viewer.Providers != null);

            var actionItems = (
                from injectorProvider in viewer.Providers
                where injectorProvider.SupportsInjectionComment
                let node =
                    injectorProvider.LocateInjectNodeByTreeOffset(provider.PsiFile, provider.Document, provider.DocumentSelection.StartOffset.Offset)
                where node != null && injectorProvider.IsSupportedLiteralForInjection(node)
                let languageInjectorProviderInLiterals = injectorProvider as ILanguageInjectorProviderInLiterals
                where (languageInjectorProviderInLiterals == null || !languageType.IsLanguage(languageInjectorProviderInLiterals.ProvidedLanguage)) &&
                    languageType.IsLanguage(injectorProvider.SupportedOriginalLanguage)
                orderby injectorProvider.Priority
                select new InjectLanguageActionItem(injectorProvider, node)).ToList();

            var languageEqualsCommentTexts = new HashSet<string>(
                from actionItem in actionItems select actionItem.AssertNotNull().InjectorProvider.LanguageEqualsCommentTexts.AssertNotNull()[0],
                StringComparer.OrdinalIgnoreCase);

            foreach (var actionItem in actionItems)
            {
                Debug.Assert(actionItem != null);

                actionItem.LanguageEqualsCommentTexts = languageEqualsCommentTexts;

                yield return actionItem.ToContextActionIntention(
                    submenuAnchor,
                    actionItem.InjectorProvider.Icon ?? PsiFeaturesUnsortedThemedIcons.LangInjection.Id);
            }
        }
    }
}