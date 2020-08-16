using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.DelegateInvoke
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.CodeRedundancy,
        "Redundant '" + nameof(Action.Invoke) + "' expression" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
        OverlapResolve = OverlapResolveKind.DEADCODE)]
    public sealed class RedundantDelegateInvokeSuggestion : Highlighting
    {
        const string SeverityId = "RedundantDelegateInvoke";

        internal RedundantDelegateInvokeSuggestion([NotNull] string message, [NotNull] IReferenceExpression referenceExpression) : base(message)
            => ReferenceExpression = referenceExpression;

        [NotNull]
        internal IReferenceExpression ReferenceExpression { get; }

        public override DocumentRange CalculateRange()
        {
            Debug.Assert(ReferenceExpression.NameIdentifier != null);

            var dotToken = ReferenceExpression.NameIdentifier.GetPreviousMeaningfulToken();
            return ReferenceExpression.NameIdentifier.GetDocumentRange().JoinLeft(dotToken.GetDocumentRange());
        }
    }
}