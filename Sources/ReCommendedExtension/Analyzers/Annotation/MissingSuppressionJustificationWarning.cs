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
public sealed class MissingSuppressionJustificationWarning(
    IAttributesOwnerDeclaration attributesOwnerDeclaration,
    IAttribute attribute,
    string message) : AttributeHighlighting(attributesOwnerDeclaration, attribute, false, message)
{
    const string SeverityId = "MissingSuppressionJustification";
}