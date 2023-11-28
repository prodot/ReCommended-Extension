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
public sealed record MissingNotNullWhenAnnotationSuggestion : Highlighting
{
    const string SeverityId = "MissingNotNullWhenAnnotation";

    internal MissingNotNullWhenAnnotationSuggestion(IAttributesOwnerDeclaration declaration, string message) : base(message)
        => Declaration = declaration;

    internal IAttributesOwnerDeclaration Declaration { get; }

    public override DocumentRange CalculateRange() => Declaration.GetNameDocumentRange();
}