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
public sealed class NotAllowedAnnotationWarning(string message, IAttributesOwnerDeclaration attributesOwnerDeclaration, IAttribute attribute)
    : AttributeHighlighting(message, attributesOwnerDeclaration, attribute, false)
{
    const string SeverityId = "NotAllowedAnnotation";
}