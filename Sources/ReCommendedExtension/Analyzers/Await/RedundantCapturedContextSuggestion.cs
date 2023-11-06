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
public sealed record RedundantCapturedContextSuggestion : Highlighting
{
    const string SeverityId = "RedundantCapturedContext";

    readonly IAwaitExpression awaitExpression;

    internal RedundantCapturedContextSuggestion(string message, IAwaitExpression awaitExpression) : base(message)
        => this.awaitExpression = awaitExpression;

    public override DocumentRange CalculateRange() => awaitExpression.AwaitKeyword.GetDocumentRange();
}