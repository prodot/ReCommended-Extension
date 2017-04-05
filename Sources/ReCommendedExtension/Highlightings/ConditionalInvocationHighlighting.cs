using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(ConditionalInvocationHighlighting.SeverityId, null, HighlightingGroupIds.CodeInfo,
        "Method invocation will be skipped if the specific condition is not defined" + ZoneMarker.Suffix, "", Severity.HINT)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name, AttributeId = HighlightingAttributeIds.DEADCODE_ATTRIBUTE,
        OverlapResolve = OverlapResolveKind.DEADCODE)]
    public sealed class ConditionalInvocationHighlighting : Highlighting
    {
        internal const string SeverityId = "ConditionalInvocation";

        [NotNull]
        readonly IInvocationExpression invocationExpression;

        internal ConditionalInvocationHighlighting(
            [NotNull] string message,
            [NotNull] IInvocationExpression invocationExpression) : base(message) => this.invocationExpression = invocationExpression;

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