using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

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
public sealed record ConditionalAnnotationHint : AttributeHighlighting
{
    const string SeverityId = "ConditionalAnnotation";

    internal ConditionalAnnotationHint(IAttributesOwnerDeclaration attributesOwnerDeclaration, IAttribute attribute, string message) : base(
        attributesOwnerDeclaration,
        attribute,
        true,
        message) { }
}