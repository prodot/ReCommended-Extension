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
        "Region contains a single element" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class RegionWithSingleElementSuggestion : RegionHighlighting
    {
        const string SeverityId = "RegionWithSingleElement";

        internal RegionWithSingleElementSuggestion([NotNull] string message, [NotNull] IStartRegion startRegion) : base(message, startRegion) { }
    }
}