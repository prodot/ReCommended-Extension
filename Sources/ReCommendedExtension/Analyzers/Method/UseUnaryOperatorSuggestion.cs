using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Method;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Use unary operator" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseUnaryOperatorSuggestion(string message, IInvocationExpression invocationExpression, string operand, string op) : Highlighting(
    message)
{
    const string SeverityId = "UseUnaryOperator";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal string Operand => operand;

    internal string Operator => op;

    public override DocumentRange CalculateRange() => invocationExpression.GetDocumentRange();
}