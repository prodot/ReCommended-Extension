using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use another argument" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseOtherArgumentSuggestion(string message, ICSharpArgument argument, string? parameterName, string replacement) : Highlighting(
    message)
{
    const string SeverityId = "UseOtherArgument";

    internal ICSharpArgument Argument => argument;

    internal string? ParameterName => parameterName;

    internal string Replacement => replacement;

    public override DocumentRange CalculateRange() => argument.Value.GetDocumentRange();
}