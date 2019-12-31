using JetBrains.Annotations;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.Annotation;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        ConditionalAnnotationHint.SeverityId,
        null,
        HighlightingGroupIds.CodeInfo,
        "Attribute will be ignored if the specific condition is not defined" + ZoneMarker.Suffix,
        "",
        Severity.HINT)]

namespace ReCommendedExtension.Analyzers.Annotation
{
    [ConfigurableSeverityHighlighting(
        SeverityId,
        CSharpLanguage.Name,
        AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
        OverlapResolve = OverlapResolveKind.DEADCODE)]
    public sealed class ConditionalAnnotationHint : AttributeHighlighting
    {
        internal const string SeverityId = "ConditionalAnnotation";

        internal ConditionalAnnotationHint(
            [NotNull] IAttributesOwnerDeclaration attributesOwnerDeclaration,
            [NotNull] IAttribute attribute,
            [NotNull] string message) : base(attributesOwnerDeclaration, attribute, true, message) { }
    }
}