using System;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.QuickFixes
{
    [QuickFix]
    public sealed class RemoveAsyncFix : QuickFixBase
    {
        [NotNull]
        readonly AsyncVoidFunctionExpressionHighlighting highlighting;

        public RemoveAsyncFix([NotNull] AsyncVoidFunctionExpressionHighlighting highlighting) => this.highlighting = highlighting;

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache) => true;

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                highlighting.RemoveAsyncModifier();
            }

            return _ => { };
        }

        public override string Text => "Remove 'async' modifier";
    }
}