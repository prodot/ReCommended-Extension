using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ValueTask;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    "Blocking on value task with 'GetAwaiter().GetResult()' might not block" + ZoneMarker.Suffix,
    "Blocking on value task with 'GetAwaiter().GetResult()' might not block",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class IntentionalBlockingAttemptWarning(
    string message,
    ICSharpExpression expression,
    ICSharpExpression valueTaskExpression,
    IReferenceExpression getAwaiterReferenceExpression,
    IReferenceExpression getResultReferenceExpression) : Highlighting(message)
{
    const string SeverityId = "IntentionalBlockingAttempt";

    internal ICSharpExpression Expression { get; } = expression;

    internal ICSharpExpression ValueTaskExpression { get; } = valueTaskExpression;

    public override DocumentRange CalculateRange()
    {
        var documentRange = getAwaiterReferenceExpression.NameIdentifier.GetDocumentRange();

        var leftParenthesis = getResultReferenceExpression.NameIdentifier.GetNextMeaningfulToken();
        if (leftParenthesis?.GetTokenType().TokenRepresentation == "(")
        {
            var rightParenthesis = leftParenthesis.GetNextMeaningfulToken();
            if (rightParenthesis?.GetTokenType().TokenRepresentation == ")")
            {
                return documentRange.JoinRight(rightParenthesis.GetDocumentRange());
            }
        }

        return documentRange.JoinRight(getResultReferenceExpression.NameIdentifier.GetDocumentRange());
    }
}