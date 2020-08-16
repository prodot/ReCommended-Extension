using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ControlFlow
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.CodeRedundancy,
        "Redundant assertion statement" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class RedundantAssertionStatementSuggestion : RedundantAssertionSuggestion
    {
        const string SeverityId = "RedundantAssertionStatement";

        [NotNull]
        readonly AssertionStatement assertion;

        internal RedundantAssertionStatementSuggestion([NotNull] string message, [NotNull] AssertionStatement assertion) : base(message)
            => this.assertion = assertion;

        internal override Assertion Assertion => assertion;

        public override DocumentRange CalculateRange() => assertion.Statement.GetDocumentRange();
    }
}