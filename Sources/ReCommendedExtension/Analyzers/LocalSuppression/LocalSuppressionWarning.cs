using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;
using ReCommendedExtension.Analyzers.LocalSuppression;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        LocalSuppressionWarning.SeverityId,
        null,
        HighlightingGroupIds.BestPractice,
        "Avoid local suppression" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Analyzers.LocalSuppression
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class LocalSuppressionWarning : Highlighting
    {
        internal const string SeverityId = "LocalSuppression";

        [NotNull]
        readonly ICommentNode comment;

        [NonNegativeValue]
        readonly int leadingWhitespaceCharacterCount;

        readonly bool justOnce;

        internal LocalSuppressionWarning(
            [NotNull] string message,
            [NotNull] ICommentNode comment,
            [NonNegativeValue] int leadingWhitespaceCharacterCount,
            bool justOnce) : base(message)
        {
            this.comment = comment;
            this.leadingWhitespaceCharacterCount = leadingWhitespaceCharacterCount;
            this.justOnce = justOnce;
        }

        public override DocumentRange CalculateRange()
        {
            var startOffset = comment.GetCommentRange().StartOffset.Offset + leadingWhitespaceCharacterCount;

            var endOffset = justOnce ? startOffset + "ReSharper disable once".Length : startOffset + "ReSharper disable".Length;

            var documentRange = comment.GetDocumentRange();

            Debug.Assert(documentRange.Document != null);

            return new DocumentRange(documentRange.Document, new TextRange(startOffset, endOffset));
        }
    }
}