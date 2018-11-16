using System;
using System.Diagnostics;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        RedundantDelegateInvokeHighlighting.SeverityId,
        null,
        HighlightingGroupIds.CodeRedundancy,
        "Redundant '" + nameof(Action.Invoke) + "' expression" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        AttributeId = HighlightingAttributeIds.DEADCODE_ATTRIBUTE,
        OverlapResolve = OverlapResolveKind.DEADCODE)]
    public sealed class RedundantDelegateInvokeHighlighting : Highlighting
    {
        internal const string SeverityId = "RedundantDelegateInvoke";

        internal RedundantDelegateInvokeHighlighting([NotNull] string message, [NotNull] IReferenceExpression referenceExpression) : base(message)
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