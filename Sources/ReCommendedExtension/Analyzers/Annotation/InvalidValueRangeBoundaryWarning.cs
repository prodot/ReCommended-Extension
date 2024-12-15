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
    string message,
    ICSharpExpression positionParameter,
    ValueRangeBoundary boundary,
    IType type,
    bool typeIsSigned) : Highlighting(message)
{
    const string SeverityId = "InvalidValueRangeBoundary";

    internal ICSharpExpression PositionParameter => positionParameter;

    internal ValueRangeBoundary Boundary => boundary;

    internal IType Type => type;

    internal bool TypeIsSigned => typeIsSigned;

    public override DocumentRange CalculateRange() => PositionParameter.GetNavigationRange();
}