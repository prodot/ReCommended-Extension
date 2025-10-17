using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Argument;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use another argument range" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseOtherArgumentRangeSuggestion(
    string message,
    IReadOnlyList<ArgumentReplacement> argumentReplacements,
    ICSharpArgument? redundantArgument) : MultipleHighlightings(message)
{
    const string SeverityId = "UseOtherArgumentRange";

    internal IReadOnlyList<ArgumentReplacement> ArgumentReplacements => argumentReplacements;

    internal ICSharpArgument? RedundantArgument => redundantArgument;
}