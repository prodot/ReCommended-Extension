using System;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using ReCommendedExtension.Assertions;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.QuickFixes
{
    [QuickFix]
    public sealed class RemoveAssertionStatementFix : QuickFixBase
    {
        [NotNull]
        readonly RedundantAssertionStatementHighlighting highlighting;

        public RemoveAssertionStatementFix([NotNull] RedundantAssertionStatementHighlighting highlighting)
        {
            this.highlighting = highlighting;
        }

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache) => true;

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var statement = ((AssertionStatement)highlighting.Assertion).Statement;

                var nextToken = statement.NextTokens().SkipWhile(token => token.IsWhitespaceToken()).FirstOrDefault();

                ModificationUtil.DeleteChildRange(
                    statement,
                    nextToken != null && nextToken.GetTokenType() == CSharpTokenType.SEMICOLON ? nextToken : (ITreeNode)statement);
            }

            return _ => { };
        }

        public override string Text => "Remove assertion";
    }
}