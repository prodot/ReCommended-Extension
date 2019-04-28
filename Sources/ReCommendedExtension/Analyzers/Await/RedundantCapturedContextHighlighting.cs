using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension;
using ReCommendedExtension.Analyzers.Await;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        RedundantCapturedContextHighlighting.SeverityId,
        null,
        HighlightingGroupIds.BestPractice,
        "Redundant captured context (add '." + ClrMethodsNames.ConfigureAwait + "(false)')" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Analyzers.Await
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class RedundantCapturedContextHighlighting : Highlighting
    {
        internal const string SeverityId = "RedundantCapturedContext";

        [NotNull]
        readonly IAwaitExpression awaitExpression;

        internal RedundantCapturedContextHighlighting([NotNull] string message, [NotNull] IAwaitExpression awaitExpression) : base(message)
            => this.awaitExpression = awaitExpression;

        public override DocumentRange CalculateRange() => awaitExpression.AwaitKeyword.GetDocumentRange();
    }
}