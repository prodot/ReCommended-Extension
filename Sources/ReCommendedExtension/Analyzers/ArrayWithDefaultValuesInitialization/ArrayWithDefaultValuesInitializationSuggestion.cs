using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.ArrayWithDefaultValuesInitialization;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use 'new T[n]' for arrays with default values" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class ArrayWithDefaultValuesInitializationSuggestion(string message, string? suggestedCode, IArrayInitializer arrayInitializer)
    : Highlighting(message)
{
    const string SeverityId = "ArrayWithDefaultValuesInitialization";

    internal string? SuggestedCode { get; } = suggestedCode;

    internal IArrayInitializer ArrayInitializer { get; } = arrayInitializer;

    public override DocumentRange CalculateRange() => ArrayInitializer.GetDocumentRange();
}