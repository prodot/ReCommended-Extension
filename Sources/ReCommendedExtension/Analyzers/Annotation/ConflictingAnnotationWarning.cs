using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    "Annotation conflicts with another annotation" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record ConflictingAnnotationWarning : AttributeHighlighting
{
    const string SeverityId = "ConflictingAnnotation";

    internal ConflictingAnnotationWarning(IAttributesOwnerDeclaration attributesOwnerDeclaration, IAttribute attribute, string message) : base(
        attributesOwnerDeclaration,
        attribute,
        false,
        message) { }
}