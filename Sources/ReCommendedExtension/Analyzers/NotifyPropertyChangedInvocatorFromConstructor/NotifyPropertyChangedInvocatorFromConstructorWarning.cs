using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "Redundant invocation of the property change notifiers from the constructor" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record NotifyPropertyChangedInvocatorFromConstructorWarning : Highlighting
{
    const string SeverityId = "NotifyPropertyChangedInvocatorFromConstructor";

    internal NotifyPropertyChangedInvocatorFromConstructorWarning(IInvocationExpression invocationExpression, string message) : base(message)
        => InvocationExpression = invocationExpression;

    internal IInvocationExpression InvocationExpression { get; }

    public override DocumentRange CalculateRange() => InvocationExpression.GetHighlightingRange();
}