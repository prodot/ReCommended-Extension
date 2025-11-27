using JetBrains.DocumentModel;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Annotation;

public abstract class AttributeHighlighting(string message, bool includeAttributeBracketsInRange) : Highlighting(message)
{
    public required IAttributesOwnerDeclaration AttributesOwnerDeclaration { get; init; }

    public required IAttribute Attribute { get; init; }

    public override DocumentRange CalculateRange()
    {
        var range = Attribute.GetDocumentRange();

        if (includeAttributeBracketsInRange
            && Attribute.GetPreviousNonWhitespaceToken() is { } previousToken
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