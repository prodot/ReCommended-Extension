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
public sealed class MissingAnnotationWarning(string message, IAttributesOwnerDeclaration declaration) : Highlighting(message)
{
    const string SeverityId = "MissingAnnotation";

    public override DocumentRange CalculateRange() => declaration.GetNameDocumentRange();
}