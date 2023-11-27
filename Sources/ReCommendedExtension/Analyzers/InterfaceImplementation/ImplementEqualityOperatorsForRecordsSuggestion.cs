using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Implement IEqualityOperators<T, T, bool> for records" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record ImplementEqualityOperatorsForRecordsSuggestion : ImplementOperatorsSuggestion
{
    const string SeverityId = "ImplementEqualityOperatorsForRecords";

    internal ImplementEqualityOperatorsForRecordsSuggestion(string message, IRecordDeclaration declaration) : base(message, declaration) { }
}