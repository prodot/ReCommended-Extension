using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.ThrowExceptionInUnexpectedLocation;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        ThrowExceptionInUnexpectedLocationWarning.SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "Exception should never be thrown in unexpected locations" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Analyzers.ThrowExceptionInUnexpectedLocation
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class ThrowExceptionInUnexpectedLocationWarning : Highlighting
    {
        internal const string SeverityId = "ThrowExceptionInUnexpectedLocation";

        [NotNull]
        readonly ICSharpTreeNode thrownStatementOrExpression;

        internal ThrowExceptionInUnexpectedLocationWarning([NotNull] string message, [NotNull] ICSharpTreeNode thrownStatementOrExpression) :
            base(message)
            => this.thrownStatementOrExpression = thrownStatementOrExpression;

        public override DocumentRange CalculateRange() => thrownStatementOrExpression.GetDocumentRange();
    }
}