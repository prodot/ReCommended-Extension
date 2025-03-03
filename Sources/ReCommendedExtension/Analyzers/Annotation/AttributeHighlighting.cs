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

        if (includeAttributeBracketsInRange
            && attribute.GetPreviousNonWhitespaceToken() is { } previousToken
            && Attribute.GetNextNonWhitespaceToken() is { } nextToken
            && previousToken.GetTokenType() == CSharpTokenType.LBRACKET
            && nextToken.GetTokenType() == CSharpTokenType.RBRACKET)
        {
            range = new DocumentRange(
                range.Document,
                new TextRange(previousToken.GetDocumentRange().TextRange.StartOffset, nextToken.GetDocumentRange().TextRange.EndOffset));
        }

        return range;
    }
}