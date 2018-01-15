using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.QuickFixes
{
    [QuickFix]
    public sealed class RemoveExceptionTypeDeclarationFromCatchClauseFix : QuickFixBase
    {
        [NotNull]
        readonly CatchClauseWithoutVariableHighlighting highlighting;

        public RemoveExceptionTypeDeclarationFromCatchClauseFix([NotNull] CatchClauseWithoutVariableHighlighting highlighting)
            => this.highlighting = highlighting;

        public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache) => true;

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                Debug.Assert(highlighting.CatchClause.LPar != null);
                Debug.Assert(highlighting.CatchClause.RPar != null);

                ModificationUtil.DeleteChildRange(highlighting.CatchClause.LPar, highlighting.CatchClause.RPar);
            }

            return _ => { };
        }

        public override string Text => "Remove exception type";
    }
}