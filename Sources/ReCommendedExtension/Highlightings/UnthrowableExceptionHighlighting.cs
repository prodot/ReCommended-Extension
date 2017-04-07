using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(UnthrowableExceptionHighlighting.SeverityId, null, HighlightingGroupIds.BestPractice,
        "Exception should never be thrown" + ZoneMarker.Suffix, "", Severity.WARNING)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class UnthrowableExceptionHighlighting : Highlighting
    {
        internal const string SeverityId = "UnthrowableException";

        [NotNull]
        readonly ICSharpExpression thrownStatementExpression;

        internal UnthrowableExceptionHighlighting(
            [NotNull] string message,
            [NotNull] ICSharpExpression thrownStatementExpression) : base(message) => this.thrownStatementExpression = thrownStatementExpression;

        public override DocumentRange CalculateRange() => thrownStatementExpression.GetDocumentRange();
    }
}