using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        ThrowExceptionInUnexpectedLocationHighlighting.SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "Exception should never be thrown in unexpected locations" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class ThrowExceptionInUnexpectedLocationHighlighting : Highlighting
    {
        internal const string SeverityId = "ThrowExceptionInUnexpectedLocation";

        [NotNull]
        readonly ICSharpTreeNode thrownStatementOrExpression;

        internal ThrowExceptionInUnexpectedLocationHighlighting([NotNull] string message, [NotNull] ICSharpTreeNode thrownStatementOrExpression) :
            base(message)
            => this.thrownStatementOrExpression = thrownStatementOrExpression;

        public override DocumentRange CalculateRange() => thrownStatementOrExpression.GetDocumentRange();
    }
}