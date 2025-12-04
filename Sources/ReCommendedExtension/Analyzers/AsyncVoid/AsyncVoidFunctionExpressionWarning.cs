using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.AsyncVoid;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    "Async void function expression" + ZoneMarker.Suffix,
    "'async void' lambda or anonymous method expression not used as a direct event handler.",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class AsyncVoidFunctionExpressionWarning(string message, ITokenNode asyncKeyword) : Highlighting(message)
{
    const string SeverityId = "AsyncVoidFunctionExpression";

    public required IAnonymousFunctionExpression AnonymousFunctionExpression { get; init; }

    public override DocumentRange CalculateRange() => asyncKeyword.GetDocumentRange();

    [QuickFix]
    public sealed class Fix(AsyncVoidFunctionExpressionWarning highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => "Remove 'async' modifier";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                highlighting.AnonymousFunctionExpression.SetAsync(false);
            }

            return null;
        }
    }
}