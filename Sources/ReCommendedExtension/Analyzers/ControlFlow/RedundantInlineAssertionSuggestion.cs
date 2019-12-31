using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.ControlFlow;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        RedundantInlineAssertionSuggestion.SeverityId,
        null,
        HighlightingGroupIds.CodeRedundancy,
        "Redundant inline assertion expression" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Analyzers.ControlFlow
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class RedundantInlineAssertionSuggestion : RedundantAssertionSuggestion
    {
        internal const string SeverityId = "RedundantInlineAssertion";

        [NotNull]
        readonly InlineAssertion assertion;

        internal RedundantInlineAssertionSuggestion([NotNull] string message, [NotNull] InlineAssertion assertion) : base(message)
            => this.assertion = assertion;

        internal override Assertion Assertion => assertion;

        public override DocumentRange CalculateRange()
        {
            Debug.Assert(assertion.InvocationExpression.InvokedExpression is IReferenceExpression);

            return ((IReferenceExpression)assertion.InvocationExpression.InvokedExpression).NameIdentifier.GetDocumentRange();
        }
    }
}