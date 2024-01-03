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
    IArrayCreationExpression expression,
    IReferenceExpression? methodReferenceToSetInferredTypeArguments) : Highlighting(message)
{
    const string SeverityId = "UseTargetTypedCollectionExpression"; // a collection expression is always target-typed (needed a distinguished id)

    internal IArrayCreationExpression Expression { get; } = expression;

    internal IReferenceExpression? MethodReferenceToSetInferredTypeArguments { get; } = methodReferenceToSetInferredTypeArguments;

    public override DocumentRange CalculateRange()
    {
        if (Expression.NewKeyword is { })
        {
            if (Expression.Dims is [{ } rankSpecifier])
            {
                return new DocumentRange(Expression.NewKeyword.GetDocumentStartOffset(), rankSpecifier.GetDocumentEndOffset());
            }

            if (Expression.RBracket is { NodeType: TokenNodeType { TokenRepresentation: "]" } })
            {
                return new DocumentRange(Expression.NewKeyword.GetDocumentStartOffset(), Expression.RBracket.GetDocumentEndOffset());
            }
        }

        return Expression.GetDocumentRange();
    }
}