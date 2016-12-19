using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(UncatchableExceptionHighlighting.SeverityId, null, HighlightingGroupIds.CodeSmell,
        "Exception should never be caught" + ZoneMarker.Suffix, "", Severity.WARNING)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class UncatchableExceptionHighlighting : Highlighting
    {
        internal const string SeverityId = "UncatchableException";

        [NotNull]
        readonly ISpecificCatchClause catchClause;

        internal UncatchableExceptionHighlighting([NotNull] string message, [NotNull] ISpecificCatchClause catchClause)
            : base(message)
        {
            this.catchClause = catchClause;
        }

        public override DocumentRange CalculateRange() => catchClause.ExceptionTypeUsage.GetDocumentRange();
    }
}