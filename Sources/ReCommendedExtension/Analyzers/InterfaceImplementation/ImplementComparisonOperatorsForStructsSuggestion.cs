using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Implement IComparisonOperators<T, T, bool> for structs" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record ImplementComparisonOperatorsForStructsSuggestion : ImplementOperatorsSuggestion
{
    const string SeverityId = "ImplementComparisonOperatorsForStructs";

    internal ImplementComparisonOperatorsForStructsSuggestion(string message, IStructDeclaration declaration) : base(message, declaration) { }
}