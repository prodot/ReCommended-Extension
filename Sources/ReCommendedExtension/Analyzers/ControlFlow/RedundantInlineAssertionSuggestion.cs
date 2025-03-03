using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ControlFlow;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "Redundant inline assertion expression" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class RedundantInlineAssertionSuggestion(string message, InlineAssertion assertion) : RedundantAssertionSuggestion(message)
{
    const string SeverityId = "RedundantInlineAssertion";

    internal override Assertion Assertion => assertion;

    public override DocumentRange CalculateRange()
        => ((IReferenceExpression)assertion.InvocationExpression.InvokedExpression).NameIdentifier.GetDocumentRange();
}