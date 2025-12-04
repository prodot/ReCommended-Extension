using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.CatchClauseWithoutVariable;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "Redundant declaration without exception variable" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class CatchClauseWithoutVariableHint(string message) : Highlighting(message)
{
    const string SeverityId = "CatchClauseWithoutVariable";

    public required ISpecificCatchClause CatchClause { get; init; }

    public override DocumentRange CalculateRange()
        => new(
            CatchClause.GetDocumentRange().Document,
            new TextRange(CatchClause.LPar.GetDocumentStartOffset().Offset, CatchClause.RPar.GetDocumentEndOffset().Offset));

    [QuickFix]
    public sealed class Fix(CatchClauseWithoutVariableHint highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Remove exception type";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                ModificationUtil.DeleteChildRange(highlighting.CatchClause.LPar, highlighting.CatchClause.RPar);
            }

            return null;
        }
    }
}