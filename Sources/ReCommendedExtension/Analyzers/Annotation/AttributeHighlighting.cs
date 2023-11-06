using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Annotation;

public abstract record AttributeHighlighting : Highlighting
{
    readonly bool includeAttributeBracketsInRange;

    private protected AttributeHighlighting(
        IAttributesOwnerDeclaration attributesOwnerDeclaration,
        IAttribute attribute,
        bool includeAttributeBracketsInRange,
        string message) : base(message)
    {
        AttributesOwnerDeclaration = attributesOwnerDeclaration;
        Attribute = attribute;

        this.includeAttributeBracketsInRange = includeAttributeBracketsInRange;
    }

    internal IAttributesOwnerDeclaration AttributesOwnerDeclaration { get; }

    internal IAttribute Attribute { get; }

    public sealed override DocumentRange CalculateRange()
    {
        var range = Attribute.GetDocumentRange();

        if (includeAttributeBracketsInRange)
        {
            var previousToken = Attribute.PrevTokens().SkipWhile(token => token.IsWhitespaceToken()).FirstOrDefault();
            var nextToken = Attribute.NextTokens().SkipWhile(token => token.IsWhitespaceToken()).FirstOrDefault();

            if (previousToken is { }
                && nextToken is { }
                && previousToken.GetTokenType() == CSharpTokenType.LBRACKET
                && nextToken.GetTokenType() == CSharpTokenType.RBRACKET)
            {
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