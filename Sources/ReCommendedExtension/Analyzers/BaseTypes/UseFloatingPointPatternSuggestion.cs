using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use pattern with floating-point constants" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseFloatingPointPatternSuggestion(
    string message,
    IInvocationExpression invocationExpression,
    ICSharpExpression expression,
    string pattern) : Highlighting(message)
{
    const string SeverityId = "UseFloatingPointPattern";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal ICSharpExpression Expression => expression;

    internal string Pattern => pattern;

    public override DocumentRange CalculateRange() => invocationExpression.GetDocumentRange();
}