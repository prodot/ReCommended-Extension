using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeInfo,
    "Attribute will be ignored if the specific condition is not defined" + ZoneMarker.Suffix,
    "",
    Severity.HINT)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class ConditionalAnnotationHint(string message) : AttributeHighlighting(message, true)
{
    const string SeverityId = "ConditionalAnnotation";
}