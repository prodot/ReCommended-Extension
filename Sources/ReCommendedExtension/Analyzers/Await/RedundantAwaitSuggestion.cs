using System;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Await
{
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
    public sealed class RedundantAwaitSuggestion : Highlighting
    {
        const string SeverityId = "RedundantAwait";

        internal RedundantAwaitSuggestion(
            [NotNull] string message,
            [NotNull] Action removeAsync,
            [NotNull] IAwaitExpression awaitExpression,
            [CanBeNull] IExpressionStatement statementToBeReplacedWithReturnStatement,
            [NotNull] ICSharpExpression expressionToReturn,
            [CanBeNull] IAttributesOwnerDeclaration attributesOwnerDeclaration) : base(message)
        {
            RemoveAsync = removeAsync;
            AwaitExpression = awaitExpression;
            StatementToBeReplacedWithReturnStatement = statementToBeReplacedWithReturnStatement;
            ExpressionToReturn = expressionToReturn;
            AttributesOwnerDeclaration = attributesOwnerDeclaration;
        }

        [NotNull]
        internal Action RemoveAsync { get; }

        [NotNull]
        internal IAwaitExpression AwaitExpression { get; }

        [CanBeNull]
        internal IExpressionStatement StatementToBeReplacedWithReturnStatement { get; }

        [CanBeNull]
        internal ICSharpExpression ExpressionToReturn { get; }

        [CanBeNull]
        internal IAttributesOwnerDeclaration AttributesOwnerDeclaration { get; }

        internal bool QuickFixRemovesConfigureAwait => AwaitExpression.Task != ExpressionToReturn;

        public override DocumentRange CalculateRange() => throw new NotSupportedException();
    }
}