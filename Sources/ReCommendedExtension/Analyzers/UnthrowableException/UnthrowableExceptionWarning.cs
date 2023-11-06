using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.UnthrowableException;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.BestPractice,
    "Exception should never be thrown" + ZoneMarker.Suffix,
    "",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed record UnthrowableExceptionWarning : Highlighting
{
    const string SeverityId = "UnthrowableException";

    readonly ICSharpExpression thrownStatementExpression;

    internal UnthrowableExceptionWarning(string message, ICSharpExpression thrownStatementExpression) : base(message)
        => this.thrownStatementExpression = thrownStatementExpression;

    public override DocumentRange CalculateRange() => thrownStatementExpression.GetDocumentRange();
}