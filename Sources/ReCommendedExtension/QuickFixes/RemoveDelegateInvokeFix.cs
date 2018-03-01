using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.QuickFixes
{
    [QuickFix]
    public sealed class RemoveDelegateInvokeFix : QuickFixBase
    {
        [NotNull]
        readonly RedundantDelegateInvokeHighlighting highlighting;

        public RemoveDelegateInvokeFix([NotNull] RedundantDelegateInvokeHighlighting highlighting) => this.highlighting = highlighting;

        public override bool IsAvailable(IUserDataHolder cache) => true;

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                Debug.Assert(highlighting.ReferenceExpression.NameIdentifier != null);

                var dotToken = highlighting.ReferenceExpression.NameIdentifier.GetPreviousMeaningfulToken();
                Debug.Assert(dotToken != null);

                ModificationUtil.DeleteChildRange(dotToken, highlighting.ReferenceExpression.NameIdentifier);
            }

            return _ => { };
        }

        public override string Text => $"Remove '{nameof(Action.Invoke)}'";
    }
}