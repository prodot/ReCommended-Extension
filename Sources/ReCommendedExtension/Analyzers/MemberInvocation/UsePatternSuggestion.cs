using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.MemberInvocation.Rules;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use pattern" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UsePatternSuggestion(string message, IInvocationExpression invocationExpression, PatternReplacement replacement) : Highlighting(
    message)
{
    const string SeverityId = "UsePattern";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal PatternReplacement Replacement => replacement;

    public override DocumentRange CalculateRange() => invocationExpression.GetDocumentRange();
}