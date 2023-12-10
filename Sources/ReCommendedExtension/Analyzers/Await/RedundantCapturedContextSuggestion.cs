using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Await;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    $"Redundant captured context (add '.{nameof(Task.ConfigureAwait)}(false)')" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class RedundantCapturedContextSuggestion(string message, IAwaitExpression awaitExpression) : Highlighting(message)
{
    const string SeverityId = "RedundantCapturedContext";

    public override DocumentRange CalculateRange() => awaitExpression.AwaitKeyword.GetDocumentRange();
}