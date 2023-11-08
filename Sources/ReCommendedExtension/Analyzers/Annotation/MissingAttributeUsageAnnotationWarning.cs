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
public sealed record MissingAttributeUsageAnnotationWarning : Highlighting
{
    const string SeverityId = "MissingAttributeUsageAnnotation";

    internal MissingAttributeUsageAnnotationWarning(IAttributesOwnerDeclaration declaration, string message) : base(message)
        => Declaration = declaration;

    internal IAttributesOwnerDeclaration Declaration { get; }

    public override DocumentRange CalculateRange() => Declaration.GetNameDocumentRange();
}