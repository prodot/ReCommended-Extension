using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.LocalSuppression;

[ElementProblemAnalyzer(typeof(ICSharpCommentNode), HighlightingTypes = new[] { typeof(LocalSuppressionWarning) })]
public sealed class LocalSuppressionAnalyzer : ElementProblemAnalyzer<ICSharpCommentNode>
{
    [Pure]
    [NonNegativeValue]
    static int GetLeadingWhitespaceCharacterCount(string commentText)
    {
        var leadingWhitespaceCharacters = 0;

        foreach (var c in commentText)
        {
            if (c is ' ' or '\t')
            {
                leadingWhitespaceCharacters++;
            }
            else
            {
                break;
            }
        }

        return leadingWhitespaceCharacters;
    }

    protected override void Run(ICSharpCommentNode element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
    {
        if (element.GetContainingFunctionLikeDeclarationOrClosure() is { })
        {
            var leadingWhitespaceCharacterCount = GetLeadingWhitespaceCharacterCount(element.CommentText);

            var commentText = element.CommentText[leadingWhitespaceCharacterCount..];

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