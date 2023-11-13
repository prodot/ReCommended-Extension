using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.InterfaceImplementation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Implement IEqualityOperators<TSelf, TOther, TResult>" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record ImplementEqualityOperatorsSuggestion : Highlighting
{
    const string SeverityId = "ImplementEqualityOperators";

    internal ImplementEqualityOperatorsSuggestion(string message, IClassLikeDeclaration declaration, bool operatorsAvailable) : base(message)
    {
        Declaration = declaration;
        OperatorsAvailable = operatorsAvailable;
    }

    internal IClassLikeDeclaration Declaration { get; }

    internal bool OperatorsAvailable { get; }

    public override DocumentRange CalculateRange() => Declaration.NameIdentifier.GetDocumentRange();
}