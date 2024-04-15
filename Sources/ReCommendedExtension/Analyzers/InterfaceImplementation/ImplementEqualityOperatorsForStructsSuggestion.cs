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
public sealed class ImplementEqualityOperatorsForStructsSuggestion(string message, IStructDeclaration declaration) : ImplementOperatorsHighlighting(
    message,
    declaration)
{
    const string SeverityId = "ImplementEqualityOperatorsForStructs";
}