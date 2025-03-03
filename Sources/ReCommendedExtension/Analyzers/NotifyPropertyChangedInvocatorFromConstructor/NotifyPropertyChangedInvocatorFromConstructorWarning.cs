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
public sealed class NotifyPropertyChangedInvocatorFromConstructorWarning(string message, IInvocationExpression invocationExpression) : Highlighting(message)
{
    const string SeverityId = "NotifyPropertyChangedInvocatorFromConstructor";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    public override DocumentRange CalculateRange() => InvocationExpression.GetHighlightingRange();
}