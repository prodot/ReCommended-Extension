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
    "Use the string property" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseStringPropertySuggestion(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression,
    string propertyName) : Highlighting(message)
{
    const string SeverityId = "UseStringProperty";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal IReferenceExpression InvokedExpression => invokedExpression;

    internal string PropertyName => propertyName;

    public override DocumentRange CalculateRange()
        => invocationExpression.GetDocumentRange().SetStartTo(invokedExpression.Reference.GetDocumentRange().StartOffset);
}