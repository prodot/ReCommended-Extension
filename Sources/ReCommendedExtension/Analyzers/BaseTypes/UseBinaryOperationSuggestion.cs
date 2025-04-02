using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use binary operation" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseBinaryOperationSuggestion(
    string message,
    IInvocationExpression invocationExpression,
    string @operator,
    ICSharpExpression leftOperand,
    ICSharpExpression rightOperand) : Highlighting(message)
{
    const string SeverityId = "UseBinaryOperation";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal string Operator => @operator;

    internal ICSharpExpression LeftOperand => leftOperand;

    internal ICSharpExpression RightOperand => rightOperand;

    public override DocumentRange CalculateRange() => invocationExpression.GetDocumentRange();
}