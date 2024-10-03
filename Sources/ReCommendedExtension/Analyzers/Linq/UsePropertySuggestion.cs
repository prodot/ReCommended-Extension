using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Linq;

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.LanguageUsage, "Use property" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UsePropertySuggestion(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression,
    string propertyName) : LinqHighlighting(message, invocationExpression, invokedExpression)
{
    const string SeverityId = "UseProperty";

    internal string? PropertyName { get; } = propertyName;
}