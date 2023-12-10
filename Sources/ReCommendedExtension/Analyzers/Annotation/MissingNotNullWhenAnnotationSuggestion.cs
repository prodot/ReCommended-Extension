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
    "Missing annotation that the parameter is not null when the method returns true" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class MissingNotNullWhenAnnotationSuggestion(IAttributesOwnerDeclaration declaration, string message) : Highlighting(message)
{
    const string SeverityId = "MissingNotNullWhenAnnotation";

    internal IAttributesOwnerDeclaration Declaration { get; } = declaration;

    public override DocumentRange CalculateRange() => Declaration.GetNameDocumentRange();
}