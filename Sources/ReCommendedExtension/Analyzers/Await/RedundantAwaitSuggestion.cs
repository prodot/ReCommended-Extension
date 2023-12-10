using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Await;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "Redundant 'await'" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
[Obsolete] // todo: remove
public sealed record RedundantAwaitSuggestion : Highlighting
{
    const string SeverityId = "RedundantAwait";

    internal RedundantAwaitSuggestion(
        string message,
        Action removeAsync,
        IAwaitExpression awaitExpression,
        IExpressionStatement? statementToBeReplacedWithReturnStatement,
        ICSharpExpression expressionToReturn,
        IAttributesOwnerDeclaration? attributesOwnerDeclaration,
        string? configureAwaitArgument) : base(message)
    {
        RemoveAsync = removeAsync;
        AwaitExpression = awaitExpression;
        StatementToBeReplacedWithReturnStatement = statementToBeReplacedWithReturnStatement;
        ExpressionToReturn = expressionToReturn;
        AttributesOwnerDeclaration = attributesOwnerDeclaration;
        ConfigureAwaitArgument = configureAwaitArgument;
    }

    internal Action RemoveAsync { get; }

    internal IAwaitExpression AwaitExpression { get; }

    internal IExpressionStatement? StatementToBeReplacedWithReturnStatement { get; }

    internal ICSharpExpression? ExpressionToReturn { get; }

    internal IAttributesOwnerDeclaration? AttributesOwnerDeclaration { get; }

    internal string? ConfigureAwaitArgument { get; }

    public override DocumentRange CalculateRange() => throw new NotSupportedException();
}