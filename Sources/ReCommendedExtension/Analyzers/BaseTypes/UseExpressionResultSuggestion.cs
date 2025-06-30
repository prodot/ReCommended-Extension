using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes;

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
    ICSharpTreeNode expression,
    string replacement,
    string? alternativeReplacement = null) : Highlighting(message)
{
    const string SeverityId = "UseExpressionResult";

    internal ICSharpTreeNode Expression => expression;

    internal string Replacement => replacement;

    internal string? AlternativeReplacement => alternativeReplacement;

    public override DocumentRange CalculateRange() => expression.GetDocumentRange();
}