using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.Region;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        EmptyRegionSuggestion.SeverityId,
        null,
        HighlightingGroupIds.DeclarationRedundancy,
        "Region is empty" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Analyzers.Region
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class EmptyRegionSuggestion : RegionHighlighting
    {
        internal const string SeverityId = "EmptyRegion";

        internal EmptyRegionSuggestion([NotNull] string message, [NotNull] IStartRegion startRegion) : base(message, startRegion) { }
    }
}