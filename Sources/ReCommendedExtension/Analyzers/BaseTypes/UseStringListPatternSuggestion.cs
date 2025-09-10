using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.LanguageUsage, "Use list pattern" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseStringListPatternSuggestion(
    string message,
    IInvocationExpression invocationExpression,
    IReferenceExpression invokedExpression,
    ListPatternSuggestionKind kind,
    (string valueArgument, bool isConstant)[] arguments,
    IBinaryExpression? binaryExpression = null) : Highlighting(message)
{
    const string SeverityId = "UseStringListPattern";

    internal IInvocationExpression InvocationExpression => invocationExpression;

    internal IReferenceExpression InvokedExpression => invokedExpression;

    internal IBinaryExpression? BinaryExpression => binaryExpression;

    internal ListPatternSuggestionKind Kind => kind;

    internal (string valueArgument, bool isConstant)[] Arguments => arguments;

    public override DocumentRange CalculateRange()
    {
        var startOffset = invokedExpression.Reference.GetDocumentRange().StartOffset;

        return (binaryExpression as ITreeNode ?? invocationExpression).GetDocumentRange().SetStartTo(startOffset);
    }
}