using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Collection;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use '[]' for empty collections" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseCollectionExpressionForEmptyInitializationWarning(string message, IArrayCreationExpression expression) : Highlighting(message)
{
    const string SeverityId = "UseCollectionExpressionForEmptyInitialization";

    internal IArrayCreationExpression Expression { get; } = expression;

    public override DocumentRange CalculateRange() => Expression.GetDocumentRange();
}