using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Missing suppression justification" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record MissingSuppressionJustificationWarning : AttributeHighlighting
{
    const string SeverityId = "MissingSuppressionJustification";

    internal MissingSuppressionJustificationWarning(IAttributesOwnerDeclaration attributesOwnerDeclaration, IAttribute attribute, string message) :
        base(attributesOwnerDeclaration, attribute, false, message) { }
}