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
    HighlightingGroupIds.CodeSmell,
    "Suspicious format specifier" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class SuspiciousFormatSpecifierWarning : Highlighting
{
    const string SeverityId = "SuspiciousFormatSpecifier";

    readonly ICSharpArgument? formatArgument;
    readonly IInterpolatedStringInsert? insert;
    readonly ICSharpLiteralExpression? formatStringExpression;
    readonly FormatStringParser.FormatItem? formatItem;

    internal SuspiciousFormatSpecifierWarning(string message, ICSharpArgument formatArgument) : base(message) => this.formatArgument = formatArgument;

    internal SuspiciousFormatSpecifierWarning(string message, IInterpolatedStringInsert insert) : base(message) => this.insert = insert;

    internal SuspiciousFormatSpecifierWarning(
        string message,
        ICSharpLiteralExpression formatStringExpression,
        FormatStringParser.FormatItem formatItem) : base(message)
    {
        this.formatStringExpression = formatStringExpression;
        this.formatItem = formatItem;
    }

    public override DocumentRange CalculateRange()
    {
        if (formatArgument is { })
        {
            return formatArgument.Value.GetDocumentRange();
        }

        if (insert is { })
        {
            return insert.FormatSpecifier.GetDocumentRange().ExtendLeft(-1); // to exclude the ':' character
        }

        if (formatStringExpression is { } && formatItem is { })
        {
            var documentRange = formatStringExpression.GetDocumentRange();

            return new DocumentRange(
                documentRange.Document,
                new TextRange(
                    documentRange.StartOffset.Offset + formatItem.FormatStringRange.StartOffset, // the ':' character already excluded
                    documentRange.StartOffset.Offset + formatItem.FormatStringRange.StartOffset + formatItem.FormatStringRange.Length));
        }

        throw new NotSupportedException();
    }
}