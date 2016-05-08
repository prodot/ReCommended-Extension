using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(NotifyPropertyChangedInvocatorFromConstructorHighlighting.SeverityId, null, HighlightingGroupIds.CodeRedundancy,
        "Redundant invocation of the property change notifiers from the constructor" + ZoneMarker.Suffix, "", Severity.WARNING, false)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name, OverlapResolve = OverlapResolveKind.DEADCODE)]
    public sealed class NotifyPropertyChangedInvocatorFromConstructorHighlighting : Highlighting
    {
        internal const string SeverityId = "NotifyPropertyChangedInvocatorFromConstructor";

        internal NotifyPropertyChangedInvocatorFromConstructorHighlighting(
            [NotNull] IInvocationExpression invocationExpression,
            [NotNull] string message) : base(message)
        {
            InvocationExpression = invocationExpression;
        }

        [NotNull]
        internal IInvocationExpression InvocationExpression { get; }

        public override DocumentRange CalculateRange() => InvocationExpression.GetHighlightingRange();
    }
}