using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Formatter;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "The format precision specifier is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantFormatPrecisionSpecifierHint(string message) : Highlighting(message)
{
    const string SeverityId = "RedundantFormatPrecisionSpecifier";

    public required FormatElement FormatElement { get; init; }

    public override DocumentRange CalculateRange()
    {
        switch (FormatElement)
        {
            case { Insert: { } insert }: return insert.FormatSpecifier.GetDocumentRange().ExtendLeft(-2); // to exclude the ':' character and the format specifier (one character)

            case { FormatStringExpression: { } formatStringExpression, FormatItem: { } formatItem }:
                var documentRange = formatStringExpression.GetDocumentRange();

                return new DocumentRange(
                    documentRange.Document,
                    new TextRange(
                        documentRange.StartOffset.Offset + formatItem.FormatStringRange.StartOffset + 1, // to exclude the format specifier (one character)
                        documentRange.StartOffset.Offset + formatItem.FormatStringRange.StartOffset + formatItem.FormatStringRange.Length));

            case { Argument: { } argument }:
                var range = argument.Value.GetDocumentRange();

                if (argument.Value is ICSharpLiteralExpression or IInterpolatedStringExpression)
                {
                    var expression = argument.Value.GetText();
                    Debug.Assert(expression.Length >= 4);

                    var format = argument.Value.AsStringConstant;
                    Debug.Assert(format is { Length: >= 2 });

                    var formatSpecifier = 0;
                    var digits = 0;

                    for (var i = 1; i < expression.Length - 1; i++)
                    {
                        if (formatSpecifier == 0)
                        {
                            if (expression[i] == format[0])
                            {
                                formatSpecifier = i;
                            }
                        }
                        else
                        {
                            if (expression[i] is >= '0' and <= '9')
                            {
                                digits++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    Debug.Assert(formatSpecifier > 0);
                    Debug.Assert(digits > 0);

                    range = range.ExtendLeft(-1 - formatSpecifier);
                    return new DocumentRange(range.Document, new TextRange(range.TextRange.StartOffset, range.TextRange.StartOffset + digits));
                }

                return range;
        }

        throw new NotSupportedException();
    }

    [QuickFix]
    public sealed class Fix(RedundantFormatPrecisionSpecifierHint highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache)
            => highlighting.FormatElement is { Insert: { } }
                or { FormatStringExpression: { }, FormatItem: { } }
                or { Argument.Value: ICSharpLiteralExpression or IInterpolatedStringExpression };

        public override string Text => "Remove format precision specifier";

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
            => _ =>
            {
                using (WriteLockCookie.Create())
                {
                    var documentRange = highlighting.CalculateRange();

                    documentRange.Document.DeleteText(documentRange.TextRange);
                }
            };
    }
}