using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Strings;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Pass the single character" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class PassSingleCharacterSuggestion(
    string message,
    ICSharpArgument argument,
    string? parameterName,
    char character,
    string? additionalArgument = null,
    ICSharpArgument? redundantArgument = null) : Highlighting(message)
{
    const string SeverityId = "PassSingleCharacter";

    internal ICSharpArgument Argument => argument;

    internal string? ParameterName => parameterName;

    internal char Character => character;

    internal string? AdditionalArgument => additionalArgument;

    internal ICSharpArgument? RedundantArgument => redundantArgument;

    public override DocumentRange CalculateRange() => argument.Value.GetDocumentRange();
}