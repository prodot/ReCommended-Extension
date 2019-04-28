using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.Region;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        RegionWithSingleElementHighlighting.SeverityId,
        null,
        HighlightingGroupIds.DeclarationRedundancy,
        "Region contains a single element" + ZoneMarker.Suffix,
        "",
        Severity.SUGGESTION)]

namespace ReCommendedExtension.Analyzers.Region
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class RegionWithSingleElementHighlighting : RegionHighlighting
    {
        internal const string SeverityId = "RegionWithSingleElement";

        internal RegionWithSingleElementHighlighting([NotNull] string message, [NotNull] IStartRegion startRegion) : base(message, startRegion) { }
    }
}