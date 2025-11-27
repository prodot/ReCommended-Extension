using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

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
    IReferenceExpression getAwaiterReferenceExpression,
    IReferenceExpression getResultReferenceExpression) : Highlighting(message)
{
    const string SeverityId = "IntentionalBlockingAttempt";

    public required ICSharpExpression Expression { get; init; }

    public required ICSharpExpression ValueTaskExpression { get; init; }

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

    [QuickFix]
    public sealed class Fix(IntentionalBlockingAttemptWarning highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Insert '.AsTask()'";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.Expression);

                ModificationUtil.ReplaceChild(
                    highlighting.Expression,
                    factory.CreateExpression("$0.AsTask().GetAwaiter().GetResult", highlighting.ValueTaskExpression));
            }

            return null;
        }
    }
}