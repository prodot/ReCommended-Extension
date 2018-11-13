using System;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        RedundantAwaitHighlighting.SeverityId,
        null,
        HighlightingGroupIds.CodeRedundancy,
        "Redundant 'await'" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        AttributeId = HighlightingAttributeIds.DEADCODE_ATTRIBUTE,
        OverlapResolve = OverlapResolveKind.DEADCODE)]
    public sealed class RedundantAwaitHighlighting : Highlighting
    {
        internal const string SeverityId = "RedundantAwait";

        internal RedundantAwaitHighlighting(
            [NotNull] string message,
            [NotNull] Action removeAsync,
            [NotNull] IAwaitExpression awaitExpression,
            bool returnStatementRequired,
            [NotNull] ICSharpExpression expressionToReturn,
            IAttributesOwnerDeclaration attributesOwnerDeclaration) : base(message)
        {
            RemoveAsync = removeAsync;
            AwaitExpression = awaitExpression;
            ReturnStatementRequired = returnStatementRequired;
            ExpressionToReturn = expressionToReturn;
            AttributesOwnerDeclaration = attributesOwnerDeclaration;
        }

        [NotNull]
        internal Action RemoveAsync { get; }

        [NotNull]
        internal IAwaitExpression AwaitExpression { get; }

        internal bool ReturnStatementRequired { get; }

        internal ICSharpExpression ExpressionToReturn { get; }

        internal IAttributesOwnerDeclaration AttributesOwnerDeclaration { get; }

        internal bool QuickFixRemovesConfigureAwait => AwaitExpression.Task != ExpressionToReturn;

        public override DocumentRange CalculateRange() => throw new NotSupportedException();
    }
}