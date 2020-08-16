using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ValueTask
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "Blocking on value task with 'GetAwaiter().GetResult()' might not block" + ZoneMarker.Suffix,
        "Blocking on value task with 'GetAwaiter().GetResult()' might not block",
        Severity.WARNING)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class IntentionalBlockingAttemptWarning : Highlighting
    {
        const string SeverityId = "IntentionalBlockingAttempt";

        [NotNull]
        readonly IReferenceExpression getAwaiterReferenceExpression;

        [NotNull]
        readonly IReferenceExpression getResultReferenceExpression;

        internal IntentionalBlockingAttemptWarning(
            [NotNull] string message,
            [NotNull] ICSharpExpression expression,
            [NotNull] ICSharpExpression valueTaskExpression,
            [NotNull] IReferenceExpression getAwaiterReferenceExpression,
            [NotNull] IReferenceExpression getResultReferenceExpression) : base(message)
        {
            this.getAwaiterReferenceExpression = getAwaiterReferenceExpression;
            this.getResultReferenceExpression = getResultReferenceExpression;

            ValueTaskExpression = valueTaskExpression;
            Expression = expression;
        }

        [NotNull]
        internal ICSharpExpression Expression { get; }

        [NotNull]
        internal ICSharpExpression ValueTaskExpression { get; }

        public override DocumentRange CalculateRange()
        {
            var documentRange = getAwaiterReferenceExpression.NameIdentifier.GetDocumentRange();

            var leftParenthesis = getResultReferenceExpression.NameIdentifier?.GetNextMeaningfulToken();
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
}