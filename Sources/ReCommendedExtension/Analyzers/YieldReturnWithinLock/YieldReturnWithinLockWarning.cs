using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.YieldReturnWithinLock;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    "'yield return' used inside the 'lock' block" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record YieldReturnWithinLockWarning : Highlighting
{
    const string SeverityId = "YieldReturnWithinLock";

    readonly IYieldStatement yieldReturnStatement;

    internal YieldReturnWithinLockWarning(string message, IYieldStatement yieldReturnStatement) : base(message)
        => this.yieldReturnStatement = yieldReturnStatement;

    public override DocumentRange CalculateRange()
        => yieldReturnStatement.YieldKeyword.GetDocumentRange().JoinRight(yieldReturnStatement.ReturnKeyword.GetDocumentRange());
}