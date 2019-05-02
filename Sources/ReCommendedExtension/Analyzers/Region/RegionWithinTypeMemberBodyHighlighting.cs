using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.Region;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        RegionWithinTypeMemberBodyHighlighting.SeverityId,
        null,
        HighlightingGroupIds.CodeStyleIssues,
        "Region is contained within a type member body" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Analyzers.Region
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class RegionWithinTypeMemberBodyHighlighting : RegionHighlighting
    {
        internal const string SeverityId = "RegionWithinTypeMemberBody";

        internal RegionWithinTypeMemberBodyHighlighting([NotNull] string message, [NotNull] IStartRegion startRegion) : base(message, startRegion) { }
    }
}