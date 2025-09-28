using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Formatter;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Pass other format specifier" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class PassOtherFormatSpecifierSuggestion(string message, FormatElement formatElement, string replacement) : Highlighting(message)
{
    const string SeverityId = "PassOtherFormatSpecifier";

    internal FormatElement FormatElement => formatElement;

    internal string Replacement => replacement;

    public override DocumentRange CalculateRange()
    {
        switch (FormatElement)
        {
            case { Insert: { } insert }: return insert.FormatSpecifier.GetDocumentRange().ExtendLeft(-1); // to exclude the ':' character

            case { FormatStringExpression: { } formatStringExpression, FormatItem: { } formatItem }:
                var documentRange = formatStringExpression.GetDocumentRange();

                return new DocumentRange(
                    documentRange.Document,
                    new TextRange(
                        documentRange.StartOffset.Offset + formatItem.FormatStringRange.StartOffset, // the ':' character already excluded
                        documentRange.StartOffset.Offset + formatItem.FormatStringRange.StartOffset + formatItem.FormatStringRange.Length));

            case { Argument: { } argument }: return argument.Value.GetDocumentRange();
        }

        throw new NotSupportedException();
    }
}