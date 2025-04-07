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
    "Use char range pattern" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseCharRangePatternSuggestion(
    string message,
    IInvocationExpression invocationExpression,
    ICSharpExpression expression,
    CharRange[] ranges) : Highlighting(message)
{
    const string SeverityId = "UseCharRangePattern";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal ICSharpExpression Expression => expression;

    internal CharRange[] Ranges => ranges;

    public override DocumentRange CalculateRange() => invocationExpression.GetDocumentRange();
}