using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
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
public sealed class CatchClauseWithoutVariableHint(string message, ISpecificCatchClause catchClause) : Highlighting(message)
{
    const string SeverityId = "CatchClauseWithoutVariable";

    internal ISpecificCatchClause CatchClause => catchClause;

    public override DocumentRange CalculateRange()
        => new(
            CatchClause.GetDocumentRange().Document,
            new TextRange(CatchClause.LPar.GetDocumentStartOffset().Offset, CatchClause.RPar.GetDocumentEndOffset().Offset));
}