using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.ConstraintViolation,
    "Annotation is not allowed" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class NotAllowedAnnotationWarning(IAttributesOwnerDeclaration attributesOwnerDeclaration, IAttribute attribute, string message)
    : AttributeHighlighting(attributesOwnerDeclaration, attribute, false, message)
{
    const string SeverityId = "NotAllowedAnnotation";
}