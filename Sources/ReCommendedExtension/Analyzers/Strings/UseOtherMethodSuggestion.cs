using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Strings;

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.BestPractice, "Use another method" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseOtherMethodSuggestion(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression,
    string otherMethodName,
    bool isNegated,
    string[] arguments,
    IBinaryExpression? binaryExpression = null) : Highlighting(message)
{
    const string SeverityId = "UseOtherMethod";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal IReferenceExpression InvokedExpression => invokedExpression;

    internal string OtherMethodName => otherMethodName;

    internal bool IsNegated => isNegated;

    internal string[] Arguments => arguments;

    internal IBinaryExpression? BinaryExpression => binaryExpression;

    public override DocumentRange CalculateRange()
    {
        var startOffset = invokedExpression.Reference.GetDocumentRange().StartOffset;

        return (BinaryExpression as ITreeNode ?? invocationExpression).GetDocumentRange().SetStartTo(startOffset);
    }
}