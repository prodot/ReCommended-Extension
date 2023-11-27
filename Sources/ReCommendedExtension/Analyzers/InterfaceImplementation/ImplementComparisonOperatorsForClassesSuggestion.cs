using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Implement IComparisonOperators<T, T, bool> for classes" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record ImplementComparisonOperatorsForClassesSuggestion : ImplementOperatorsSuggestion
{
    const string SeverityId = "ImplementComparisonOperatorsForClasses";

    internal ImplementComparisonOperatorsForClassesSuggestion(string message, IClassDeclaration declaration) : base(message, declaration) { }
}