using JetBrains.DocumentModel;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Resolve;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Analyzers.Strings;

[RegisterConfigurableSeverity(SeverityId, null, HighlightingGroupIds.LanguageUsage, "Use list pattern" + ZoneMarker.Suffix, "", Severity.SUGGESTION)]
[ConfigurableSeverityHighlighting(SeverityId, CSharpLanguage.Name)]
public sealed class UseStringListPatternSuggestion : Highlighting
{
    const string SeverityId = "UseStringListPattern";

    UseStringListPatternSuggestion(
        string message,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ListPatternSuggestionKind kind,
        IBinaryExpression? binaryExpression) : base(message)
    {
        InvocationExpression = invocationExpression;
        InvokedExpression = invokedExpression;
        Kind = kind;
        BinaryExpression = binaryExpression;
    }

    public UseStringListPatternSuggestion(
        string message,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ListPatternSuggestionKind kind,
        char[] characters,
        IBinaryExpression? binaryExpression = null) : this(message, invocationExpression, invokedExpression, kind, binaryExpression)
        => Characters = characters;

    public UseStringListPatternSuggestion(
        string message,
        IInvocationExpression invocationExpression,
        IReferenceExpression invokedExpression,
        ListPatternSuggestionKind kind,
        string valueArgument,
        IBinaryExpression? binaryExpression = null) : this(message, invocationExpression, invokedExpression, kind, binaryExpression)
        => ValueArgument = valueArgument;

    internal IInvocationExpression InvocationExpression { get; }

    internal IReferenceExpression InvokedExpression { get; }

    internal IBinaryExpression? BinaryExpression { get; }

    internal char[]? Characters { get; }

    internal ListPatternSuggestionKind Kind { get; }

    internal string? ValueArgument { get; }

    public override DocumentRange CalculateRange()
    {
        var startOffset = InvokedExpression.Reference.GetDocumentRange().StartOffset;

        return (BinaryExpression as ITreeNode ?? InvocationExpression).GetDocumentRange().SetStartTo(startOffset);
    }
}