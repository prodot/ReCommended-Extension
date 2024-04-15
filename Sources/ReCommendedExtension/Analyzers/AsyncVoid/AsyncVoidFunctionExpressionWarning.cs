using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.AsyncVoid;

[RegisterConfigurableSeverity(
    SeverityId,
    null,
    HighlightingGroupIds.CodeSmell,
    "Async void function expression" + ZoneMarker.Suffix,
    "'async void' lambda or anonymous method expression not used as a direct event handler.",
    Severity.WARNING)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class AsyncVoidFunctionExpressionWarning(string message, ITokenNode asyncKeyword, Action removeAsyncModifier) : Highlighting(message)
{
    const string SeverityId = "AsyncVoidFunctionExpression";

    internal void RemoveAsyncModifier() => removeAsyncModifier();

    public override DocumentRange CalculateRange() => asyncKeyword.GetDocumentRange();
}