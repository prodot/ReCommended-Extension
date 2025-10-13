using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Argument;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use another argument" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseOtherArgumentSuggestion(
    string message,
    ICSharpArgument argument,
    ParameterKind parameterKind,
    string? parameterName,
    string replacement,
    string? additionalArgument,
    string? additionalArgumentParameterName,
    ICSharpArgument? redundantArgument) : Highlighting(message)
{
    const string SeverityId = "UseOtherArgument";

    internal ICSharpArgument Argument => argument;

    internal ParameterKind ParameterKind => parameterKind;

    internal string? ParameterName => parameterName;

    internal string Replacement => replacement;

    internal string? AdditionalArgument => additionalArgument;

    internal string? AdditionalArgumentParameterName => additionalArgumentParameterName;

    internal ICSharpArgument? RedundantArgument => redundantArgument;

    public override DocumentRange CalculateRange() => argument.Value.GetDocumentRange();
}