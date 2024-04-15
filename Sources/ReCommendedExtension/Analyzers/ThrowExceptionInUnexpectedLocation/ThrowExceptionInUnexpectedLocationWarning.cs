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
public sealed class ThrowExceptionInUnexpectedLocationWarning(string message, ICSharpTreeNode thrownStatementOrExpression) : Highlighting(message)
{
    const string SeverityId = "ThrowExceptionInUnexpectedLocation";

    public override DocumentRange CalculateRange() => thrownStatementOrExpression.GetDocumentRange();
}