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
    HighlightingGroupIds.BestPractice,
    "Use binary operator" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseBinaryOperatorSuggestion(
    string message,
    IInvocationExpression invocationExpression,
    string @operator,
    string leftOperand,
    string rightOperand,
    IReferenceExpression? invokedExpression = null) : Highlighting(message)
{
    const string SeverityId = "UseBinaryOperator";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal string Operator => @operator;

    internal string LeftOperand => leftOperand;

    internal string RightOperand => rightOperand;

    public override DocumentRange CalculateRange()
    {
        var documentRange = invocationExpression.GetDocumentRange();

        if (invokedExpression is { })
        {
            return documentRange.SetStartTo(invokedExpression.Reference.GetDocumentRange().StartOffset);
        }

        return documentRange;
    }
}