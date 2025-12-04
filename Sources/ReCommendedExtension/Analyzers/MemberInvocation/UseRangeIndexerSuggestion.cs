using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Analyzers.MemberInvocation.Rules;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use indexer or range indexer" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseRangeIndexerSuggestion(string message) : Highlighting(message)
{
    const string SeverityId = "UseRangeIndexer";

    public required IInvocationExpression InvocationExpression { get; init; }

    public required IReferenceExpression InvokedExpression { get; init; }

    public required RangeIndexerReplacement Replacement { get; init; }

    public override DocumentRange CalculateRange()
        => InvocationExpression.GetDocumentRange().SetStartTo(InvokedExpression.Reference.GetDocumentRange().StartOffset);

    [QuickFix]
    public sealed class Fix(UseRangeIndexerSuggestion highlighting) : QuickFixBase
    {
        public override bool IsAvailable(IUserDataHolder cache) => true;

        public override string Text
        {
            get
            {
                var otherException = highlighting.Replacement.CanThrowOtherException ? " (other exception could be thrown)" : "";

                return $"Replace with '[{highlighting.Replacement.IndexDisplayText.TrimToSingleLineWithMaxLength(120)}]'{otherException}";
            }
        }

        protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

                var conditionalAccess = highlighting.InvokedExpression.HasConditionalAccessSign ? "?" : "";

                var expression = ModificationUtil.ReplaceChild(
                    highlighting.InvocationExpression,
                    factory.CreateExpression(
                        $"$0{conditionalAccess}[{highlighting.Replacement.Index}]",
                        highlighting.InvokedExpression.QualifierExpression));

                if (expression is IElementAccessExpression { Arguments: [{ Value: IRangeExpression rangeExpression }] })
                {
                    rangeExpression.LeftOperand?.TryRemoveParentheses(factory);
                    rangeExpression.RightOperand?.TryRemoveParentheses(factory);
                }
            }

            return null;
        }
    }
}