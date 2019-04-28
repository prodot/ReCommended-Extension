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

namespace ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor
{
    [QuickFix]
    public sealed class RemoveNotifyPropertyChangedInvocatorFix : QuickFixBase
    {
        [NotNull]
        readonly NotifyPropertyChangedInvocatorFromConstructorHighlighting highlighting;

        public RemoveNotifyPropertyChangedInvocatorFix([NotNull] NotifyPropertyChangedInvocatorFromConstructorHighlighting highlighting)
            => this.highlighting = highlighting;

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache) => true;

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var nextToken = highlighting.InvocationExpression.NextTokens().SkipWhile(token => token.IsWhitespaceToken()).FirstOrDefault();

                ModificationUtil.DeleteChildRange(
                    highlighting.InvocationExpression,
                    nextToken != null && nextToken.GetTokenType() == CSharpTokenType.SEMICOLON
                        ? nextToken
                        : (ITreeNode)highlighting.InvocationExpression);
            }

            return _ => { };
        }

        public override string Text => "Remove invocation";
    }
}