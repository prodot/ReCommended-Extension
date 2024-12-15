using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Linq;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use collection property" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseCollectionPropertySuggestion(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression,
    string propertyName) : LinqHighlighting(message, invocationExpression, invokedExpression)
{
    const string SeverityId = "UseCollectionProperty";

    internal string PropertyName => propertyName;
}