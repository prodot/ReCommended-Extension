using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ConditionalInvocation
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.CodeInfo,
        "Method invocation will be skipped if the specific condition is not defined" + ZoneMarker.Suffix,
        "",
        Severity.HINT)]
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
        OverlapResolve = OverlapResolveKind.DEADCODE)]
    public sealed class ConditionalInvocationHint : Highlighting
    {
        const string SeverityId = "ConditionalInvocation";

        [NotNull]
        readonly IInvocationExpression invocationExpression;

        internal ConditionalInvocationHint([NotNull] string message, [NotNull] IInvocationExpression invocationExpression) : base(message)
            => this.invocationExpression = invocationExpression;

        public override DocumentRange CalculateRange()
        {
            var range = invocationExpression.GetHighlightingRange();

            var nextToken = invocationExpression.NextTokens().SkipWhile(token => token.IsWhitespaceToken()).FirstOrDefault();

            if (nextToken != null && nextToken.GetTokenType() == CSharpTokenType.SEMICOLON)
            {
                Debug.Assert(range.Document != null);

                range = new DocumentRange(
                    range.Document,
                    new JetBrains.Util.TextRange(range.TextRange.StartOffset, nextToken.GetDocumentRange().TextRange.EndOffset));
            }

            return range;
        }
    }
}