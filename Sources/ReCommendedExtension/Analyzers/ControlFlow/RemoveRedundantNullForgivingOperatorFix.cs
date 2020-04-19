using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.ControlFlow
{
    [QuickFix]
    public sealed class RemoveRedundantNullForgivingOperatorFix : QuickFixBase
    {
        [NotNull]
        readonly RedundantNullForgivingOperatorSuggestion highlighting;

        public RemoveRedundantNullForgivingOperatorFix([NotNull] RedundantNullForgivingOperatorSuggestion highlighting)
            => this.highlighting = highlighting;

        public override bool IsAvailable(IUserDataHolder cache) => true;

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var operatorSign = ((NullForgivingOperation)highlighting.Assertion).SuppressNullableWarningExpression.OperatorSign;
                Debug.Assert(operatorSign != null);

                ModificationUtil.DeleteChild(operatorSign);
            }

            return _ => { };
        }

        public override string Text => "Remove null-forgiving operator";
    }
}