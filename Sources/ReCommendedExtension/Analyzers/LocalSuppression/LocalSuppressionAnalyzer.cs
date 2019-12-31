using System;
using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.LocalSuppression
{
    [ElementProblemAnalyzer(typeof(ICSharpCommentNode), HighlightingTypes = new[] { typeof(LocalSuppressionWarning) })]
    public sealed class LocalSuppressionAnalyzer : ElementProblemAnalyzer<ICSharpCommentNode>
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

                    default: return leadingWhitespaceCharacters;
                }
            }

            return leadingWhitespaceCharacters;
        }

        protected override void Run(ICSharpCommentNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (element.GetContainingFunctionLikeDeclarationOrClosure() == null)
            {
                return;
            }

            var leadingWhitespaceCharacterCount = GetLeadingWhitespaceCharacterCount(element.CommentText);

            var commentText = element.CommentText.Remove(0, leadingWhitespaceCharacterCount);

            if (commentText.StartsWith("ReSharper disable once", StringComparison.Ordinal))
            {
                consumer.AddHighlighting(new LocalSuppressionWarning("Avoid local suppression.", element, leadingWhitespaceCharacterCount, true));
            }
            else
            {
                if (commentText.StartsWith("ReSharper disable", StringComparison.Ordinal))
                {
                    consumer.AddHighlighting(
                        new LocalSuppressionWarning("Avoid local suppression.", element, leadingWhitespaceCharacterCount, false));
                }
            }
        }
    }
}