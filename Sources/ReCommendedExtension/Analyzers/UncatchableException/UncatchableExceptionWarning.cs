using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.UncatchableException;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    "Exception should never be caught" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record UncatchableExceptionWarning : Highlighting
{
    const string SeverityId = "UncatchableException";

    readonly ISpecificCatchClause catchClause;

    internal UncatchableExceptionWarning(string message, ISpecificCatchClause catchClause) : base(message) => this.catchClause = catchClause;

    public override DocumentRange CalculateRange() => catchClause.ExceptionTypeUsage.GetDocumentRange();
}