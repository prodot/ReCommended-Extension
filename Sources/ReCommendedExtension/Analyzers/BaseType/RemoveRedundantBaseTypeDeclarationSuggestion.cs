using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.BaseType;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Redundant base type declaration" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RemoveRedundantBaseTypeDeclarationSuggestion(string message, IExtendsList baseTypes) : Highlighting(message)
{
    const string SeverityId = "RemoveRedundantBaseTypeDeclaration";

    internal IExtendsList BaseTypes => baseTypes;

    public override DocumentRange CalculateRange() => baseTypes.ExtendedTypes[0].GetDocumentRange();
}