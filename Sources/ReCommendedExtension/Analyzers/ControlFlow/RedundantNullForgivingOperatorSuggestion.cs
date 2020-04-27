using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using ReCommendedExtension.Analyzers.ControlFlow;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        RedundantNullForgivingOperatorSuggestion.SeverityId,
        null,
        HighlightingGroupIds.CodeRedundancy,
        "Redundant null-forgiving operator" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Analyzers.ControlFlow
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
        OverlapResolve = OverlapResolveKind.DEADCODE)]
    public sealed class RedundantNullForgivingOperatorSuggestion : RedundantAssertionSuggestion
    {
        internal const string SeverityId = "RedundantNullForgivingOperator";

        [NotNull]
        readonly NullForgivingOperation nullForgivingOperation;

        internal RedundantNullForgivingOperatorSuggestion([NotNull] string message, [NotNull] NullForgivingOperation nullForgivingOperation) :
            base(message)
            => this.nullForgivingOperation = nullForgivingOperation;

        internal override Assertion Assertion => nullForgivingOperation;

        public override DocumentRange CalculateRange()
            => nullForgivingOperation.SuppressNullableWarningExpression.OperatorSign.GetHighlightingRange();
    }
}