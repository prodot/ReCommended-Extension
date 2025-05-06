using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes;

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
public sealed class RedundantFormatPrecisionSpecifierHint(string message, ICSharpArgument formatArgument) : Highlighting(message)
{
    const string SeverityId = "RedundantFormatPrecisionSpecifier";

    internal ICSharpArgument FormatArgument => formatArgument;

    public override DocumentRange CalculateRange()
    {
        var range = formatArgument.Value.GetDocumentRange();

        if (formatArgument.Value is ICSharpLiteralExpression or IInterpolatedStringExpression)
        {
            var format = formatArgument.Value.TryGetStringConstant();
            Debug.Assert(format is { Length: >= 2 });

            var expression = formatArgument.Value.GetText();
            Debug.Assert(expression.Length >= 4);

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
}