using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.Annotation;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        NotAllowedAnnotationHighlighting.SeverityId,
        null,
        HighlightingGroupIds.ConstraintViolation,
        "Nullability annotation is not allowed" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Analyzers.Annotation
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class NotAllowedAnnotationHighlighting : AttributeHighlighting
    {
        internal const string SeverityId = "NotAllowedAnnotation";

        internal NotAllowedAnnotationHighlighting(
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration,
            [NotNull] IAttribute attribute,
            [NotNull] string message) : base(attributesOwnerDeclaration, attribute, false, message) { }
    }
}