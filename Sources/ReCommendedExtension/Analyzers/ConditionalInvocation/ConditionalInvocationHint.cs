using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ConditionalInvocation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeInfo,
    "Method invocation will be skipped if the specific condition is not defined" + ZoneMarker.Suffix,
    "",
    Severity.HINT)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class ConditionalInvocationHint(string message, IInvocationExpression invocationExpression) : Highlighting(message)
{
    const string SeverityId = "ConditionalInvocation";

    public override DocumentRange CalculateRange()
    {
        var range = invocationExpression.GetHighlightingRange();

        if (invocationExpression.GetNextNonWhitespaceToken() is { } nextToken && nextToken.GetTokenType() == CSharpTokenType.SEMICOLON)
        {
            range = new DocumentRange(
                range.Document,
                new JetBrains.Util.TextRange(range.TextRange.StartOffset, nextToken.GetDocumentRange().TextRange.EndOffset));
        }

        return range;
    }
}