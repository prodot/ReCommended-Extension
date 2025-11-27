using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.DelegateInvoke;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    $"Redundant '{nameof(Action.Invoke)}' expression" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantDelegateInvokeHint(string message) : Highlighting(message)
{
    const string SeverityId = "RedundantDelegateInvoke";

    public required IReferenceExpression ReferenceExpression { get; init; }

    public override DocumentRange CalculateRange()
    {
        var dotToken = ReferenceExpression.NameIdentifier.GetPreviousMeaningfulToken();

        return ReferenceExpression.NameIdentifier.GetDocumentRange().JoinLeft(dotToken.GetDocumentRange());
    }

    [QuickFix]
    public sealed class Fix(RedundantDelegateInvokeHint highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text => $"Remove '{nameof(Action.Invoke)}'";

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                if (highlighting.ReferenceExpression.NameIdentifier.GetPreviousMeaningfulToken() is { } dotToken)
                {
                    ModificationUtil.DeleteChildRange(dotToken, highlighting.ReferenceExpression.NameIdentifier);
                }
            }

            return null;
        }
    }
}