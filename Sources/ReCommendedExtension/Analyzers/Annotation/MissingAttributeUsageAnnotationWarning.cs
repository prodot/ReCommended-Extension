using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Missing attribute usage annotation" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class MissingAttributeUsageAnnotationWarning(IAttributesOwnerDeclaration declaration, string message) : Highlighting(message)
{
    const string SeverityId = "MissingAttributeUsageAnnotation";

    internal IAttributesOwnerDeclaration Declaration { get; } = declaration;

    public override DocumentRange CalculateRange() => Declaration.GetNameDocumentRange();
}