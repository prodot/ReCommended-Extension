using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReCommendedExtension.Analyzers.CatchClauseWithoutVariable;

[QuickFix]
public sealed class RemoveExceptionTypeDeclarationFromCatchClauseFix : QuickFixBase
{
    readonly CatchClauseWithoutVariableSuggestion highlighting;

    public RemoveExceptionTypeDeclarationFromCatchClauseFix(CatchClauseWithoutVariableSuggestion highlighting) => this.highlighting = highlighting;

    public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache) => true;

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            ModificationUtil.DeleteChildRange(highlighting.CatchClause.LPar, highlighting.CatchClause.RPar);
        }

        return _ => { };
    }

    public override string Text => "Remove exception type";
}