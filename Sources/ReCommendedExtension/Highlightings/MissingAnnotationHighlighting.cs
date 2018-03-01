using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Highlightings;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        MissingAnnotationHighlighting.SeverityId,
        null,
        HighlightingGroupIds.ConstraintViolation,
        "Missing nullability annotation" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Highlightings
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class MissingAnnotationHighlighting : Highlighting
    {
        internal const string SeverityId = "MissingAnnotation";

        [NotNull]
        readonly IAttributesOwnerDeclaration declaration;

        internal MissingAnnotationHighlighting([NotNull] string message, [NotNull] IAttributesOwnerDeclaration declaration) : base(message)
            => this.declaration = declaration;

        public override DocumentRange CalculateRange() => declaration.GetNameDocumentRange();
    }
}