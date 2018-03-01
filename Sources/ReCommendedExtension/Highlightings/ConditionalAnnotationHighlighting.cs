using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        ConditionalAnnotationHighlighting.SeverityId,
        null,
        HighlightingGroupIds.CodeInfo,
        "Attribute will be ignored if the specific condition is not defined" + ZoneMarker.Suffix,
        "",
        Severity.HINT)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        AttributeId = HighlightingAttributeIds.DEADCODE_ATTRIBUTE,
        OverlapResolve = OverlapResolveKind.DEADCODE)]
    public sealed class ConditionalAnnotationHighlighting : AttributeHighlighting
    {
        internal const string SeverityId = "ConditionalAnnotation";

        internal ConditionalAnnotationHighlighting(
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration,
            [NotNull] IAttribute attribute,
            [NotNull] string message) : base(attributesOwnerDeclaration, attribute, true, message) { }
    }
}