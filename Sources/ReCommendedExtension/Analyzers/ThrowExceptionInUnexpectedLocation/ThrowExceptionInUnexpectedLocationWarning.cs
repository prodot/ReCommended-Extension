using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ThrowExceptionInUnexpectedLocation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    "Exception should never be thrown in unexpected locations" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record ThrowExceptionInUnexpectedLocationWarning : Highlighting
{
    const string SeverityId = "ThrowExceptionInUnexpectedLocation";

    readonly ICSharpTreeNode thrownStatementOrExpression;

    internal ThrowExceptionInUnexpectedLocationWarning(string message, ICSharpTreeNode thrownStatementOrExpression) : base(message)
        => this.thrownStatementOrExpression = thrownStatementOrExpression;

    public override DocumentRange CalculateRange() => thrownStatementOrExpression.GetDocumentRange();
}