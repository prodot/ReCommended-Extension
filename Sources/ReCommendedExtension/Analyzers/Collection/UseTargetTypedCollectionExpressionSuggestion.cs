using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Collection;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use target-typed collection expressions" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseTargetTypedCollectionExpressionSuggestion(
    string message,
    ICSharpExpression expression,
    ICSharpExpression? spreadItem,
    TreeNodeCollection<IInitializerElement>? items,
    IReferenceExpression? methodReferenceToSetInferredTypeArguments) : Highlighting(message)
{
    const string SeverityId = "UseTargetTypedCollectionExpression"; // a collection expression is always target-typed (needed a distinguished id)

    internal ICSharpExpression Expression { get; } = expression;

    internal ICSharpExpression? SpreadItem { get; } = spreadItem;

    internal TreeNodeCollection<IInitializerElement> Items { get; } = items ?? TreeNodeCollection<IInitializerElement>.Empty;

    internal IReferenceExpression? MethodReferenceToSetInferredTypeArguments { get; } = methodReferenceToSetInferredTypeArguments;

    public override DocumentRange CalculateRange()
    {
        if (Expression is ICreationExpression { NewKeyword : { } } creationExpression)
        {
            if (Expression is IArrayCreationExpression arrayCreationExpression)
            {
                if (arrayCreationExpression.Dims is [{ } rankSpecifier])
                {
                    return new DocumentRange(creationExpression.NewKeyword.GetDocumentStartOffset(), rankSpecifier.GetDocumentEndOffset());
                }

                if (arrayCreationExpression.RBracket is { NodeType: TokenNodeType { TokenRepresentation: "]" } })
                {
                    return new DocumentRange(
                        creationExpression.NewKeyword.GetDocumentStartOffset(),
                        arrayCreationExpression.RBracket.GetDocumentEndOffset());
                }
            }

            return creationExpression.NewKeyword.GetDocumentRange();
        }

        return Expression.GetDocumentRange();
    }
}