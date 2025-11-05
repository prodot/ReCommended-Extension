using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
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
public sealed class UsePatternSuggestion(
    string message,
    ICSharpExpression expression,
    IReferenceExpression invokedExpression,
    PatternReplacement replacement) : Highlighting(message)
{
    const string SeverityId = "UsePattern";

    internal ICSharpExpression Expression => expression;

    internal PatternReplacement Replacement => replacement;

    public override DocumentRange CalculateRange()
        => replacement.HighlightOnlyInvokedMethod
            ? expression.GetDocumentRange().SetStartTo(invokedExpression.Reference.GetDocumentRange().StartOffset)
            : expression.GetDocumentRange();
}