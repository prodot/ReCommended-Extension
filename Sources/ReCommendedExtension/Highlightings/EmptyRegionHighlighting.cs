using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        EmptyRegionHighlighting.SeverityId,
        null,
        HighlightingGroupIds.DeclarationRedundancy,
        "Region is empty" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class EmptyRegionHighlighting : RegionHighlighting
    {
        internal const string SeverityId = "EmptyRegion";

        internal EmptyRegionHighlighting([NotNull] string message, [NotNull] IStartRegion startRegion) : base(message, startRegion) { }
    }
}