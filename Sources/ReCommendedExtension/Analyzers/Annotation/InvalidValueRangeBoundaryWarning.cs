using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Analyzers.Annotation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.ConstraintViolation,
    "Annotation argument is out of range" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record InvalidValueRangeBoundaryWarning : Highlighting
{
    const string SeverityId = "InvalidValueRangeBoundary";

    internal InvalidValueRangeBoundaryWarning(
        ICSharpExpression positionParameter,
        ValueRangeBoundary boundary,
        IType type,
        bool typeIsSigned,
        string message) : base(message)
    {
        PositionParameter = positionParameter;
        Boundary = boundary;
        Type = type;
        TypeIsSigned = typeIsSigned;
    }

    internal ICSharpExpression PositionParameter { get; }

    internal ValueRangeBoundary Boundary { get; }

    internal IType Type { get; }

    internal bool TypeIsSigned { get; }

    public override DocumentRange CalculateRange() => PositionParameter.GetNavigationRange();
}