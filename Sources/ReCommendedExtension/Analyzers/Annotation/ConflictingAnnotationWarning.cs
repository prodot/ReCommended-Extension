using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.Annotation;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        ConflictingAnnotationWarning.SeverityId,
        null,
        HighlightingGroupIds.CodeSmell,
        "Annotation conflicts with another annotation" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Analyzers.Annotation
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class ConflictingAnnotationWarning : AttributeHighlighting
    {
        internal const string SeverityId = "ConflictingAnnotation";

        internal ConflictingAnnotationWarning(
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration,
            [NotNull] IAttribute attribute,
            [NotNull] string message) : base(attributesOwnerDeclaration, attribute, false, message) { }
    }
}