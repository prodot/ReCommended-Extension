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
public sealed record ArrayWithDefaultValuesInitializationSuggestion : Highlighting
{
    const string SeverityId = "ArrayWithDefaultValuesInitialization";

    internal ArrayWithDefaultValuesInitializationSuggestion(string message, string? suggestedCode, IArrayInitializer arrayInitializer) : base(message)
    {
        SuggestedCode = suggestedCode;
        ArrayInitializer = arrayInitializer;
    }

    internal string? SuggestedCode { get; }

    internal IArrayInitializer ArrayInitializer { get; }

    public override DocumentRange CalculateRange() => ArrayInitializer.GetDocumentRange();
}