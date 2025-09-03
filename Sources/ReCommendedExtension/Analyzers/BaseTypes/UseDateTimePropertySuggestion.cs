using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    $"Use the {nameof(DateTime)} property" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseDateTimePropertySuggestion(
    string message,
    IReferenceExpressionReference invokingReference,
    ICSharpExpression? qualifierExpression,
    IReferenceExpression referenceExpression,
    string propertyName) : Highlighting(message)
{
    const string SeverityId = "UseDateTimeProperty";

    internal IReferenceExpression ReferenceExpression => referenceExpression;

    internal ICSharpExpression? QualifierExpression => qualifierExpression;

    internal string PropertyName => propertyName;

    public override DocumentRange CalculateRange()
        => referenceExpression.GetDocumentRange().SetStartTo(invokingReference.GetDocumentRange().StartOffset);
}