using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Feature.Services.Daemon.Attributes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

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
    IReferenceExpression invokedExpression) : Highlighting(message)
{
    const string SeverityId = "RedundantMethodInvocation";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal IReferenceExpression InvokedExpression => invokedExpression;

    [Pure]
    internal bool RemoveEntireInvocationExpression()
        => invocationExpression.IsUsedAsStatement && invokedExpression.QualifierExpression is IReferenceExpression;

    public override DocumentRange CalculateRange()
    {
        if (RemoveEntireInvocationExpression())
        {
            return invocationExpression.GetDocumentRange();
        }

        var startOffset = invokedExpression.Reference.GetDocumentRange().StartOffset;

        return invocationExpression.GetDocumentRange().SetStartTo(startOffset);
    }
}