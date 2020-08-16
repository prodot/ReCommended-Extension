using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Annotation
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.ConstraintViolation,
        "Nullability annotation is not allowed" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class NotAllowedAnnotationWarning : AttributeHighlighting
    {
        const string SeverityId = "NotAllowedAnnotation";

        internal NotAllowedAnnotationWarning(
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration,
            [NotNull] IAttribute attribute,
            [NotNull] string message) : base(attributesOwnerDeclaration, attribute, false, message) { }
    }
}