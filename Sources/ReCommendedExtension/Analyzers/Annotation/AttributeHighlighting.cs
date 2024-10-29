using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Annotation;

public abstract class AttributeHighlighting(
    string message,
    IAttributesOwnerDeclaration attributesOwnerDeclaration,
    IAttribute attribute,
    bool includeAttributeBracketsInRange) : Highlighting(message)
{
    internal IAttributesOwnerDeclaration AttributesOwnerDeclaration => attributesOwnerDeclaration;

    internal IAttribute Attribute => attribute;

    public override DocumentRange CalculateRange()
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
                    new TextRange(previousToken.GetDocumentRange().TextRange.StartOffset, nextToken.GetDocumentRange().TextRange.EndOffset));
            }
        }

        return range;
    }
}