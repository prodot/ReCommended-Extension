using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeRedundancy,
    "Method invocation is redundant" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(
    SeverityId,
    CSharpLanguage.Name,
    AttributeId = AnalysisHighlightingAttributeIds.DEADCODE,
    OverlapResolve = OverlapResolveKind.DEADCODE)]
public sealed class RedundantMethodInvocationHint(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression,
    ICSharpExpression? argumentToKeep = null) : Highlighting(message)
{
    const string SeverityId = "RedundantMethodInvocation";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal IReferenceExpression InvokedExpression => invokedExpression;

    internal ICSharpExpression? ArgumentToKeep => argumentToKeep;

    [Pure]
    internal bool RemoveEntireInvocationExpression()
        => invocationExpression.IsUsedAsStatement() && invokedExpression.QualifierExpression is IReferenceExpression;

    public override DocumentRange CalculateRange()
    {
        if (RemoveEntireInvocationExpression())
        {
            return invocationExpression.GetDocumentRange();
        }

        if (argumentToKeep is { })
        {
            var endOffset = invokedExpression.Reference.GetDocumentRange().EndOffset;

            return invocationExpression.GetDocumentRange().SetEndTo(endOffset);
        }

        var startOffset = invokedExpression.Reference.GetDocumentRange().StartOffset;

        return invocationExpression.GetDocumentRange().SetStartTo(startOffset);
    }
}