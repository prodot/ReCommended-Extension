using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.ConstraintViolation,
    "Missing nullability annotation" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record MissingAnnotationWarning : Highlighting
{
    const string SeverityId = "MissingAnnotation";

    readonly IAttributesOwnerDeclaration declaration;

    internal MissingAnnotationWarning(string message, IAttributesOwnerDeclaration declaration) : base(message) => this.declaration = declaration;

    public override DocumentRange CalculateRange() => declaration.GetNameDocumentRange();
}