using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(RedundantAnnotationHighlighting.SeverityId, null, HighlightingGroupIds.DeclarationRedundancy,
        "Redundant nullability annotation" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class RedundantAnnotationHighlighting : AttributeHighlighting
    {
        internal const string SeverityId = "RedundantAnnotation";

        internal RedundantAnnotationHighlighting(
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration,
            [NotNull] IAttribute attribute,
            [NotNull] string message) : base(attributesOwnerDeclaration, attribute, false, message) { }
    }
}