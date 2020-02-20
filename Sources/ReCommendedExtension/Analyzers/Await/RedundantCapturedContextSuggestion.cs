using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.Await;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        RedundantCapturedContextSuggestion.SeverityId,
        null,
        HighlightingGroupIds.BestPractice,
        "Redundant captured context (add '." + nameof(Task.ConfigureAwait) + "(false)')" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Analyzers.Await
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class RedundantCapturedContextSuggestion : Highlighting
    {
        internal const string SeverityId = "RedundantCapturedContext";

        [NotNull]
        readonly IAwaitExpression awaitExpression;

        internal RedundantCapturedContextSuggestion([NotNull] string message, [NotNull] IAwaitExpression awaitExpression) : base(message)
            => this.awaitExpression = awaitExpression;

        public override DocumentRange CalculateRange() => awaitExpression.AwaitKeyword.GetDocumentRange();
    }
}