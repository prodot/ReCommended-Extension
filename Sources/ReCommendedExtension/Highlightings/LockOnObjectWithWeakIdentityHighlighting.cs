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
        LockOnObjectWithWeakIdentityHighlighting.SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "Lock on object with weak identity" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class LockOnObjectWithWeakIdentityHighlighting : Highlighting
    {
        internal const string SeverityId = "LockOnObjectWithWeakIdentity";

        [NotNull]
        readonly ICSharpExpression monitor;

        internal LockOnObjectWithWeakIdentityHighlighting([NotNull] string message, [NotNull] ICSharpExpression monitor) : base(message)
            => this.monitor = monitor;

        public override DocumentRange CalculateRange() => monitor.GetDocumentRange();
    }
}