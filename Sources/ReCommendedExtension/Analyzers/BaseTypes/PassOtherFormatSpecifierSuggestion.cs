using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Pass other format specifier" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class PassOtherFormatSpecifierSuggestion(string message, ICSharpArgument formatArgument, string replacement) : Highlighting(message)
{
    const string SeverityId = "PassOtherFormatSpecifier";

    internal ICSharpArgument FormatArgument => formatArgument;

    internal string Replacement => replacement;

    public override DocumentRange CalculateRange() => formatArgument.Value.GetDocumentRange();
}