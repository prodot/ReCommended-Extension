using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.DeclarationRedundancy,
    "Redundant annotation argument" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class RedundantAnnotationArgumentSuggestion(
    string message,
    IAttributesOwnerDeclaration attributesOwnerDeclaration,
    IAttribute attribute,
    ICSharpArgument argument) : AttributeHighlighting(message, attributesOwnerDeclaration, attribute, false)
{
    const string SeverityId = "RedundantAnnotationArgument";

    public override DocumentRange CalculateRange() => argument.GetDocumentRange();
}