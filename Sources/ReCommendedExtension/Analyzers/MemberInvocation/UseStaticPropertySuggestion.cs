using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.LanguageUsage,
    "Use the suggested static property" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseStaticPropertySuggestion(
    string message,
    IReferenceExpressionReference invokingReference,
    ICSharpExpression? qualifierExpression,
    IReferenceExpression referenceExpression,
    string propertyName) : Highlighting(message)
{
    const string SeverityId = "UseStaticProperty";

    internal IReferenceExpression ReferenceExpression => referenceExpression;

    internal ICSharpExpression? QualifierExpression => qualifierExpression;

    internal string PropertyName => propertyName;

    public override DocumentRange CalculateRange()
        => referenceExpression.GetDocumentRange().SetStartTo(invokingReference.GetDocumentRange().StartOffset);
}