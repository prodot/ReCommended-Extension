using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.DeclarationRedundancy,
    "Redundant nullability annotation" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class RedundantAnnotationSuggestion(IAttributesOwnerDeclaration attributesOwnerDeclaration, IAttribute attribute, string message)
    : AttributeHighlighting(attributesOwnerDeclaration, attribute, false, message)
{
    const string SeverityId = "RedundantAnnotation";
}