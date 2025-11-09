using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
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
    ArgumentReplacement argumentReplacement,
    UpcomingArgument? additionalArgument,
    ICSharpArgument? redundantArgument) : Highlighting(message)
{
    const string SeverityId = "UseOtherArgument";

    internal ArgumentReplacement ArgumentReplacement => argumentReplacement;

    internal UpcomingArgument? AdditionalArgument => additionalArgument;

    internal ICSharpArgument? RedundantArgument => redundantArgument;

    public override DocumentRange CalculateRange() => argumentReplacement.Argument.Value.GetDocumentRange();
}