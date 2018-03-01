using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Highlightings
{
    public abstract class AttributeHighlighting : Highlighting
    {
        readonly bool includeAttributeBracketsInRange;

        internal AttributeHighlighting(
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration,
            [NotNull] IAttribute attribute,
            bool includeAttributeBracketsInRange,
            [NotNull] string message) : base(message)
        {
            AttributesOwnerDeclaration = attributesOwnerDeclaration;
            Attribute = attribute;

            this.includeAttributeBracketsInRange = includeAttributeBracketsInRange;
        }

        [NotNull]
        internal IAttributesOwnerDeclaration AttributesOwnerDeclaration { get; }

        [NotNull]
        internal IAttribute Attribute { get; }

        public sealed override DocumentRange CalculateRange()
        {
            var range = Attribute.GetDocumentRange();

            if (includeAttributeBracketsInRange)
            {
                var previousToken = Attribute.PrevTokens().SkipWhile(token => token.IsWhitespaceToken()).FirstOrDefault();
                var nextToken = Attribute.NextTokens().SkipWhile(token => token.IsWhitespaceToken()).FirstOrDefault();

                if (previousToken != null &&
                    nextToken != null &&
                    previousToken.GetTokenType() == CSharpTokenType.LBRACKET &&
                    nextToken.GetTokenType() == CSharpTokenType.RBRACKET)
                {
                    Debug.Assert(range.Document != null);

                    range = new DocumentRange(
                        range.Document,
                        new JetBrains.Util.TextRange(
                            previousToken.GetDocumentRange().TextRange.StartOffset,
                            nextToken.GetDocumentRange().TextRange.EndOffset));
                }
            }

            return range;
        }
    }
}