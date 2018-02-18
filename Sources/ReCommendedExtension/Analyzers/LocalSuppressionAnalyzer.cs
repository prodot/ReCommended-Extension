using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Highlightings;

namespace ReCommendedExtension.Analyzers
{
    [ElementProblemAnalyzer(typeof(ICommentNode), HighlightingTypes = new[] { typeof(LocalSuppressionHighlighting) })]
    public sealed class LocalSuppressionAnalyzer : ElementProblemAnalyzer<ICommentNode>
    {
        static int GetLeadingWhitespaceCharacterCount([NotNull] string commentText)
        {
            var leadingWhitespaceCharacters = 0;

            for (var i = 0; i < commentText.Length; i++)
            {
                switch (commentText[i])
                {
                    case ' ':
                    case '\t':
                        leadingWhitespaceCharacters++;
                        break;

                    default:
                        return leadingWhitespaceCharacters;
                }
            }

            return leadingWhitespaceCharacters;
        }

        protected override void Run(ICommentNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            var leadingWhitespaceCharacterCount = GetLeadingWhitespaceCharacterCount(element.CommentText);

            var commentText = element.CommentText.Remove(0, leadingWhitespaceCharacterCount);

            if (commentText.StartsWith("ReSharper disable once", StringComparison.Ordinal))
            {
                consumer.AddHighlighting(
                    new LocalSuppressionHighlighting("Avoid local suppression.", element, leadingWhitespaceCharacterCount, true));
                return;
            }

            if (commentText.StartsWith("ReSharper disable All", StringComparison.Ordinal))
            {
                return;
            }

            if (commentText.StartsWith("ReSharper disable", StringComparison.Ordinal))
            {
                consumer.AddHighlighting(
                    new LocalSuppressionHighlighting("Avoid local suppression.", element, leadingWhitespaceCharacterCount, false));
            }
        }
    }
}