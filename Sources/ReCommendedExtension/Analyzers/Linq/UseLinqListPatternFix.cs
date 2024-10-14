using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Linq;

[QuickFix]
public sealed class UseLinqListPatternFix : QuickFixBase
{
    readonly LinqHighlighting highlighting;

    public UseLinqListPatternFix(UseIndexerSuggestion highlighting) => this.highlighting = highlighting;

    public UseLinqListPatternFix(UseLinqListPatternSuggestion highlighting) => this.highlighting = highlighting;

    [Pure]
    string GetReplacement(string? emptyMessage = null, string? multipleItemsMessage = null)
    {
        var throwThatEmpty = $"throw new {nameof(InvalidOperationException)}({(emptyMessage is { } ? $"\"{emptyMessage}\"" : "...")})";
        var throwThatEmptyOrHasMoreItems =
            $"throw new {nameof(InvalidOperationException)}({(multipleItemsMessage is { } ? $"\"{multipleItemsMessage}\"" : "...")})";

        return highlighting switch
        {
            UseIndexerSuggestion { IndexArgument: "0" } => $"is [var first, ..] ? first : {throwThatEmpty}",

            UseIndexerSuggestion { IndexArgument: "^1" } => $"is [.., var last] ? last : {throwThatEmpty}",

            UseLinqListPatternSuggestion { Kind: ListPatternSuggestionKind.FirstOrDefault, DefaultValueArgument: { } defaultValueArgument } =>
                $"is [var first, ..] ? first : {defaultValueArgument}",

            UseLinqListPatternSuggestion { Kind: ListPatternSuggestionKind.FirstOrDefault, DefaultValueArgument: not { } } =>
                "is [var first, ..] ? first : default",

            UseLinqListPatternSuggestion { Kind: ListPatternSuggestionKind.LastOrDefault, DefaultValueArgument: { } defaultValueArgument } =>
                $"is [.., var last] ? last : {defaultValueArgument}",

            UseLinqListPatternSuggestion { Kind: ListPatternSuggestionKind.LastOrDefault, DefaultValueArgument: not { } } =>
                "is [.., var last] ? last : default",

            UseLinqListPatternSuggestion { Kind: ListPatternSuggestionKind.Single } => $"is [var item] ? item : {throwThatEmptyOrHasMoreItems}",

            _ => throw new NotSupportedException(),
        };
    }

    public override bool IsAvailable(IUserDataHolder cache)
        => highlighting.InvocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110 // introduced list patterns
            && highlighting is UseIndexerSuggestion { IndexArgument: "0" or "^1" } or UseLinqListPatternSuggestion;

    public override string Text => $"Replace with '{GetReplacement()}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var replacement = GetReplacement("List is empty.", "List is either empty or contains more than one element.");

            ModificationUtil
                .ReplaceChild(
                    highlighting.InvocationExpression,
                    factory.CreateExpression($"($0 {replacement})", highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}