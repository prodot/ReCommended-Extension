using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ExpressionResult;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use expression result" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseExpressionResultSuggestion(
    string message,
    ICSharpInvocationInfo expression,
    ExpressionResultReplacements replacements) : Highlighting(message)
{
    const string SeverityId = "UseExpressionResult";

    internal ICSharpTreeNode Expression => (ICSharpTreeNode)expression;

    internal ExpressionResultReplacements Replacements => replacements;

    public override DocumentRange CalculateRange() => Expression.GetDocumentRange();
}