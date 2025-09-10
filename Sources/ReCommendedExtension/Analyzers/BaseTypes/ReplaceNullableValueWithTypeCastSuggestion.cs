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
    $"Replace '{nameof(Nullable<int>)}<T>.{nameof(Nullable<int>.Value)}' with a type cast" + ZoneMarker.Suffix,
    "",
    Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class ReplaceNullableValueWithTypeCastSuggestion(string message, IReferenceExpression referenceExpression) : Highlighting(message)
{
    const string SeverityId = "ReplaceNullableValueWithTypeCast";

    internal IReferenceExpression ReferenceExpression => referenceExpression;

    public override DocumentRange CalculateRange()
        => referenceExpression.GetDocumentRange().SetStartTo(referenceExpression.Reference.GetDocumentRange().StartOffset);
}