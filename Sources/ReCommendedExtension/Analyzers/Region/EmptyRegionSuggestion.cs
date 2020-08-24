using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Region
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.DeclarationRedundancy,
        "Region is empty" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class EmptyRegionSuggestion : RegionHighlighting
    {
        const string SeverityId = "EmptyRegion";

        internal EmptyRegionSuggestion([NotNull] string message, [NotNull] IStartRegion startRegion) : base(message, startRegion) { }
    }
}