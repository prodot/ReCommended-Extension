using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Implement IEqualityOperators<TSelf, TOther, TResult> for structs" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record ImplementEqualityOperatorsForStructsSuggestion : ImplementEqualityOperatorsSuggestion
{
    const string SeverityId = "ImplementEqualityOperatorsForStructs";

    internal ImplementEqualityOperatorsForStructsSuggestion(string message, IStructDeclaration declaration) : base(message, declaration) { }
}