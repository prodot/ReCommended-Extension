using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using ReCommendedExtension.Analyzers.Annotation;
using ZoneMarker = ReCommendedExtension.ZoneMarker;

[assembly:
    RegisterConfigurableSeverity(
        InvalidValueRangeBoundaryWarning.SeverityId,
        null,
        HighlightingGroupIds.ConstraintViolation,
        "Annotation argument is out of range" + ZoneMarker.Suffix,
        "",
        Severity.WARNING)]

namespace ReCommendedExtension.Analyzers.Annotation
{
    [ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
    public sealed class InvalidValueRangeBoundaryWarning : Highlighting
    {
        internal const string SeverityId = "InvalidValueRangeBoundary";

        internal InvalidValueRangeBoundaryWarning(
            [NotNull] ICSharpExpression positionParameter,
            ValueRangeBoundary boundary,
            [NotNull] IType type,
            bool typeIsSigned,
            [NotNull] string message) : base(message)
        {
            PositionParameter = positionParameter;
            Boundary = boundary;
            Type = type;
            TypeIsSigned = typeIsSigned;
        }

        [NotNull]
        internal ICSharpExpression PositionParameter { get; }

        internal ValueRangeBoundary Boundary { get; }

        [NotNull]
        internal IType Type { get; }

        internal bool TypeIsSigned { get; }

        public override DocumentRange CalculateRange() => PositionParameter.GetNavigationRange();
    }
}