using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Analyzers.Method.Rules;

namespace ReCommendedExtension.Analyzers.Method;

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
    BinaryOperatorOperands operands,
    string op,
    IReferenceExpression? invokedExpression) : Highlighting(message)
{
    const string SeverityId = "UseBinaryOperator";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal BinaryOperatorOperands Operands => operands;

    internal string Operator => op;

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