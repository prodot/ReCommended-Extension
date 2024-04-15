using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Implement IComparisonOperators<T, T, bool> for records" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class ImplementComparisonOperatorsForRecordsSuggestion(string message, IRecordDeclaration declaration) : ImplementOperatorsHighlighting(
    message,
    declaration)
{
    const string SeverityId = "ImplementComparisonOperatorsForRecords";
}