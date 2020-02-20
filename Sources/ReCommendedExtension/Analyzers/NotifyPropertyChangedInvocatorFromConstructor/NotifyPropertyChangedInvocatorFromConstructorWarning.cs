using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        NotifyPropertyChangedInvocatorFromConstructorWarning.SeverityId,
        null,
        HighlightingGroupIds.CodeRedundancy,
        "Redundant invocation of the property change notifiers from the constructor" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class NotifyPropertyChangedInvocatorFromConstructorWarning : Highlighting
    {
        internal const string SeverityId = "NotifyPropertyChangedInvocatorFromConstructor";

        internal NotifyPropertyChangedInvocatorFromConstructorWarning(
            [NotNull] IInvocationExpression invocationExpression,
            [NotNull] string message) : base(message)
            => InvocationExpression = invocationExpression;

        [NotNull]
        internal IInvocationExpression InvocationExpression { get; }

        public override DocumentRange CalculateRange() => InvocationExpression.GetHighlightingRange();
    }
}