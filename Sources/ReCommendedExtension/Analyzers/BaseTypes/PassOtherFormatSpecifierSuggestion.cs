using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Pass other format specifier" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class PassOtherFormatSpecifierSuggestion : Highlighting
{
    const string SeverityId = "PassOtherFormatSpecifier";

    internal PassOtherFormatSpecifierSuggestion(string message, ICSharpArgument formatArgument, string replacement) : base(message)
    {
        FormatArgument = formatArgument;
        Replacement = replacement;
    }

    internal PassOtherFormatSpecifierSuggestion(string message, IInterpolatedStringInsert insert, string replacement) : base(message)
    {
        Insert = insert;
        Replacement = replacement;
    }

    internal PassOtherFormatSpecifierSuggestion(
        string message,
        ICSharpLiteralExpression formatStringExpression,
        FormatStringParser.FormatItem formatItem,
        string replacement) : base(message)
    {
        FormatStringExpression = formatStringExpression;
        FormatItem = formatItem;
        Replacement = replacement;
    }

    internal ICSharpArgument? FormatArgument { get; }

    internal IInterpolatedStringInsert? Insert { get; }

    internal ICSharpLiteralExpression? FormatStringExpression { get; }

    internal FormatStringParser.FormatItem? FormatItem { get; }

    internal string Replacement { get; }

    public override DocumentRange CalculateRange()
    {
        if (FormatArgument is { })
        {
            return FormatArgument.Value.GetDocumentRange();
        }

        if (Insert is { })
        {
            return Insert.FormatSpecifier.GetDocumentRange().ExtendLeft(-1); // to exclude the ':' character
        }

        if (FormatStringExpression is { } && FormatItem is { })
        {
            var documentRange = FormatStringExpression.GetDocumentRange();

            return new DocumentRange(
                documentRange.Document,
                new TextRange(
                    documentRange.StartOffset.Offset + FormatItem.FormatStringRange.StartOffset, // the ':' character already excluded
                    documentRange.StartOffset.Offset + FormatItem.FormatStringRange.StartOffset + FormatItem.FormatStringRange.Length));
        }

        throw new NotSupportedException();
    }
}