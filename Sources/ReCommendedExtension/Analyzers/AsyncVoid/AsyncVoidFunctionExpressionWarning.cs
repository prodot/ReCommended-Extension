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
public sealed record AsyncVoidFunctionExpressionWarning : Highlighting
{
    const string SeverityId = "AsyncVoidFunctionExpression";

    readonly ITokenNode asyncKeyword;

    readonly Action removeAsyncModifier;

    internal AsyncVoidFunctionExpressionWarning(string message, ITokenNode asyncKeyword, Action removeAsyncModifier) : base(message)
    {
        this.asyncKeyword = asyncKeyword;
        this.removeAsyncModifier = removeAsyncModifier;
    }

    internal void RemoveAsyncModifier() => removeAsyncModifier();

    public override DocumentRange CalculateRange() => asyncKeyword.GetDocumentRange();
}