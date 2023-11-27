using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Implement IEqualityOperators<T, T, bool> for structs" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record ImplementEqualityOperatorsForStructsSuggestion : ImplementOperatorsSuggestion
{
    const string SeverityId = "ImplementEqualityOperatorsForStructs";

    internal ImplementEqualityOperatorsForStructsSuggestion(string message, IStructDeclaration declaration) : base(message, declaration) { }
}