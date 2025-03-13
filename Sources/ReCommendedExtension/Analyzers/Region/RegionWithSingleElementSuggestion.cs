using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Region;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.DeclarationRedundancy,
    "Region contains a single element" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class RegionWithSingleElementSuggestion(string message, IStartRegion startRegion) : RegionHighlighting(message, startRegion)
{
    const string SeverityId = "RegionWithSingleElement";
}