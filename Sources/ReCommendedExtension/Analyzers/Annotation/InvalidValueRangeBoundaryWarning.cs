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
public sealed class InvalidValueRangeBoundaryWarning(
    ICSharpExpression positionParameter,
    ValueRangeBoundary boundary,
    IType type,
    bool typeIsSigned,
    string message) : Highlighting(message)
{
    const string SeverityId = "InvalidValueRangeBoundary";

    internal ICSharpExpression PositionParameter { get; } = positionParameter;

    internal ValueRangeBoundary Boundary { get; } = boundary;

    internal IType Type { get; } = type;

    internal bool TypeIsSigned { get; } = typeIsSigned;

    public override DocumentRange CalculateRange() => PositionParameter.GetNavigationRange();
}