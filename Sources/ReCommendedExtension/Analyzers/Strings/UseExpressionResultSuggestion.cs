using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Strings;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use expression result" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseExpressionResultSuggestion(string message, IInvocationExpression invocationExpression, string replacement) : Highlighting(
    message)
{
    const string SeverityId = "UseExpressionResult";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal string Replacement => replacement;

    public override DocumentRange CalculateRange() => invocationExpression.GetDocumentRange();
}