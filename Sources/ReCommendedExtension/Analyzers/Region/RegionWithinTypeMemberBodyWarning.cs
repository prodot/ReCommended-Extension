using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Region;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeStyleIssues,
    "Region is contained within a type member body" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class RegionWithinTypeMemberBodyWarning(string message, IStartRegion startRegion) : RegionHighlighting(message, startRegion)
{
    const string SeverityId = "RegionWithinTypeMemberBody";
}