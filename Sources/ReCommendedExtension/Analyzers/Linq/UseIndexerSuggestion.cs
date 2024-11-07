using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Linq;

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.LanguageUsage, "Use indexer" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseIndexerSuggestion(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression,
    string indexArgument,
    bool canThrowOtherException) : LinqHighlighting(message, invocationExpression, invokedExpression)
{
    const string SeverityId = "UseIndexer";

    internal string IndexArgument => indexArgument;

    internal bool CanThrowOtherException => canThrowOtherException;
}