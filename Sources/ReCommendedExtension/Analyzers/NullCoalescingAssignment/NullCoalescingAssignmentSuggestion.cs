using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.NullCoalescingAssignment;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        NullCoalescingAssignmentSuggestion.SeverityId,
        null,
        HighlightingGroupIds.LanguageUsage,
        "Replace 'if' statement with compound assignment." + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Analyzers.NullCoalescingAssignment
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class NullCoalescingAssignmentSuggestion : Highlighting
    {
        internal const string SeverityId = "NullCoalescingAssignment";

        internal NullCoalescingAssignmentSuggestion(
            [NotNull] string message,
            [NotNull] IIfStatement ifStatement,
            [NotNull] IReferenceExpression assignmentDestination,
            [NotNull] ICSharpExpression assignedExpression) : base(message)
        {
            IfStatement = ifStatement;
            AssignmentDestination = assignmentDestination;
            AssignedExpression = assignedExpression;
        }

        [NotNull]
        internal IIfStatement IfStatement { get; }

        [NotNull]
        internal IReferenceExpression AssignmentDestination { get; }

        [NotNull]
        internal ICSharpExpression AssignedExpression { get; }

        public override DocumentRange CalculateRange() => IfStatement.IfKeyword.GetHighlightingRange();
    }
}