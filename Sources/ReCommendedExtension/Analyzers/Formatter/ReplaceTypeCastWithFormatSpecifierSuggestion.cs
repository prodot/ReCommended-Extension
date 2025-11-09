using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Formatter;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use a format specifier instead of the type cast" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class ReplaceTypeCastWithFormatSpecifierSuggestion(
    string message,
    IInterpolatedStringInsert insert,
    ICSharpExpression expression,
    string formatSpecifier) : Highlighting(message)
{
    const string SeverityId = "ReplaceTypeCastWithFormatSpecifier";

    internal ICSharpExpression Expression => expression;

    internal string FormatSpecifier => formatSpecifier;

    public override DocumentRange CalculateRange() => insert.GetHighlightingRange();
}