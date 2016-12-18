using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Assertions;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(RedundantAssertionStatementHighlighting.SeverityId, null, HighlightingGroupIds.CodeRedundancy,
        "Redundant assertion statement" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class RedundantAssertionStatementHighlighting : RedundantAssertionHighlighting
    {
        internal const string SeverityId = "RedundantAssertionStatement";

        [NotNull]
        readonly AssertionStatement assertion;

        internal RedundantAssertionStatementHighlighting([NotNull] string message, [NotNull] AssertionStatement assertion) : base(message)
        {
            this.assertion = assertion;
        }

        internal override Assertion Assertion => assertion;

        public override DocumentRange CalculateRange() => assertion.Statement.GetDocumentRange();
    }
}