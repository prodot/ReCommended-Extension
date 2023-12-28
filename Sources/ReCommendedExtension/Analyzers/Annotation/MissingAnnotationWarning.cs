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
    "Missing annotation" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class MissingAnnotationWarning(IAttributesOwnerDeclaration declaration, string message) : Highlighting(message)
{
    const string SeverityId = "MissingAnnotation";

    public override DocumentRange CalculateRange()
    {
        if (declaration is IPrimaryConstructorDeclaration primaryConstructor)
        {
            return primaryConstructor.GetDocumentRange();
        }

        return declaration.GetNameDocumentRange();
    }
}