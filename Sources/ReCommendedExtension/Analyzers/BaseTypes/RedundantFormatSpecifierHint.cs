using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "The format specifier is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantFormatSpecifierHint : Highlighting
{
    const string SeverityId = "RedundantFormatSpecifier";

    internal RedundantFormatSpecifierHint(string message, IInterpolatedStringInsert insert) : base(message) => Insert = insert;

    internal RedundantFormatSpecifierHint(
        string message,
        ICSharpLiteralExpression formatStringExpression,
        FormatStringParser.FormatItem formatItem) : base(message)
    {
        FormatStringExpression = formatStringExpression;
        FormatItem = formatItem;
    }

    internal IInterpolatedStringInsert? Insert { get; }

    internal ICSharpLiteralExpression? FormatStringExpression { get; }

    internal FormatStringParser.FormatItem? FormatItem { get; }

    public override DocumentRange CalculateRange()
    {
        if (Insert is { })
        {
            return Insert.FormatSpecifier.GetDocumentRange(); // the ':' character already included
        }

        if (FormatStringExpression is { } && FormatItem is { })
        {
            var documentRange = FormatStringExpression.GetDocumentRange();

            return new DocumentRange(
                documentRange.Document,
                new TextRange(
                    documentRange.StartOffset.Offset + FormatItem.FormatStringRange.StartOffset - 1, // to include the ':' character
                    documentRange.StartOffset.Offset + FormatItem.FormatStringRange.StartOffset + FormatItem.FormatStringRange.Length));
        }

        throw new NotSupportedException();
    }
}