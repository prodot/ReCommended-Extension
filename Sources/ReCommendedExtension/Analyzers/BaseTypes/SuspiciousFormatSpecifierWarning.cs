using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    "Suspicious format specifier" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class SuspiciousFormatSpecifierWarning(string message, ICSharpArgument formatArgument) : Highlighting(message)
{
    const string SeverityId = "SuspiciousFormatSpecifier";

    public override DocumentRange CalculateRange() => formatArgument.Value.GetDocumentRange();
}