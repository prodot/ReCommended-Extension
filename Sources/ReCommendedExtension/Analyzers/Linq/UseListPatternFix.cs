using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.CodeStyle.Suggestions;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Linq;

[QuickFix]
public sealed class UseListPatternFix : QuickFixBase
{
    readonly LinqHighlighting highlighting;

    public UseListPatternFix(UseIndexerSuggestion highlighting) => this.highlighting = highlighting;

    public UseListPatternFix(UseListPatternSuggestion highlighting) => this.highlighting = highlighting;

    public override bool IsAvailable(IUserDataHolder cache)
        => highlighting.InvocationExpression.GetCSharpLanguageLevel() >= CSharpLanguageLevel.CSharp110 // introduced list patterns
            && highlighting is UseIndexerSuggestion { IndexArgument: "0" or "^1" } or UseListPatternSuggestion;

    public override string Text
    {
        get
        {
            const string throwException = $"throw new {nameof(InvalidOperationException)}(...)";

            var replacement = highlighting switch
            {
                UseIndexerSuggestion { IndexArgument: "0" } => $"is [var first, ..] ? first : {throwException}",

                UseIndexerSuggestion { IndexArgument: "^1" } => $"is [.., var last] ? last : {throwException}",

                UseListPatternSuggestion { Kind: ListPatternSuggestionKind.FirstOrDefault, DefaultValueArgument: { } defaultValueArgument } =>
                    $"is [var first, ..] ? first : {defaultValueArgument}",

                UseListPatternSuggestion { Kind: ListPatternSuggestionKind.FirstOrDefault, DefaultValueArgument: not { } } =>
                    "is [var first, ..] ? first : default",

                UseListPatternSuggestion { Kind: ListPatternSuggestionKind.LastOrDefault, DefaultValueArgument: { } defaultValueArgument } =>
                    $"is [.., var last] ? last : {defaultValueArgument}",

                UseListPatternSuggestion { Kind: ListPatternSuggestionKind.LastOrDefault, DefaultValueArgument: not { } } =>
                    "is [.., var last] ? last : default",

                UseListPatternSuggestion { Kind: ListPatternSuggestionKind.Single } => $"is [var item] ? item : {throwException}",

                _ => throw new NotSupportedException(),
            };

            return $"Replace with '{replacement}'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            const string throwThatEmpty = $"""throw new {nameof(InvalidOperationException)}("List is empty.")""";
            const string throwThatEmptyOrHasMoreItems =
                $"""throw new {nameof(InvalidOperationException)}("List is either empty or contains more than one element.")""";

            var expression = ModificationUtil.ReplaceChild(
                highlighting.InvocationExpression,
                factory.CreateExpression(
                    highlighting switch
                    {
                        UseIndexerSuggestion { IndexArgument: "0" } => $"($0 is [var first, ..] ? first : {throwThatEmpty})",

                        UseIndexerSuggestion { IndexArgument: "^1" } => $"($0 is [.., var last] ? last : {throwThatEmpty})",

                        UseListPatternSuggestion { Kind: ListPatternSuggestionKind.FirstOrDefault, DefaultValueArgument: { } defaultValueArgument } =>
                            $"($0 is [var first, ..] ? first : {defaultValueArgument})",

                        UseListPatternSuggestion { Kind: ListPatternSuggestionKind.FirstOrDefault, DefaultValueArgument: not { } } =>
                            "($0 is [var first, ..] ? first : default)",

                        UseListPatternSuggestion { Kind: ListPatternSuggestionKind.LastOrDefault, DefaultValueArgument: { } defaultValueArgument } =>
                            $"($0 is [.., var last] ? last : {defaultValueArgument})",

                        UseListPatternSuggestion { Kind: ListPatternSuggestionKind.LastOrDefault, DefaultValueArgument: not { } } =>
                            "($0 is [.., var last] ? last : default)",

                        UseListPatternSuggestion { Kind: ListPatternSuggestionKind.Single } =>
                            $"($0 is [var item] ? item : {throwThatEmptyOrHasMoreItems})",

                        _ => throw new NotSupportedException(),
                    },
                    highlighting.InvokedExpression.QualifierExpression));

            if (expression is IParenthesizedExpression parenthesizedExpression
                && CodeStyleUtil.SuggestStyle<IRedundantParenthesesCodeStyleSuggestion>(expression, LanguageManager.Instance, null) is
                {
                    NeedsToRemove: true,
                })
            {
                ModificationUtil.ReplaceChild(expression, factory.CreateExpression("$0", parenthesizedExpression.Expression));
            }
        }

        return _ => { };
    }
}