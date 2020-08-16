using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Annotation
{
    [RegisterConfigurableSeverity(
        SeverityId,
        null,
        HighlightingGroupIds.ConstraintViolation,
        "Missing nullability annotation" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class MissingAnnotationWarning : Highlighting
    {
        const string SeverityId = "MissingAnnotation";

        [NotNull]
        readonly IAttributesOwnerDeclaration declaration;

        internal MissingAnnotationWarning([NotNull] string message, [NotNull] IAttributesOwnerDeclaration declaration) : base(message)
            => this.declaration = declaration;

        public override DocumentRange CalculateRange() => declaration.GetNameDocumentRange();
    }
}