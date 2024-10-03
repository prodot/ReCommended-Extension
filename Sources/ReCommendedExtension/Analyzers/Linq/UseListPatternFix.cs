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

            return highlighting switch
            {
                UseIndexerSuggestion { IndexArgument: "0" } => $"Replace with 'is [var first, ..] ? first : {throwException}'",

                UseIndexerSuggestion { IndexArgument: "^1" } => $"Replace with 'is [.., var last] ? last : {throwException}'",

                UseListPatternSuggestion { Kind: ListPatternSuggestionKind.FirstOrDefault, DefaultValueArgument: { } defaultValueArgument } =>
                    $"Replace with 'is [var first, ..] ? first : {defaultValueArgument}'",

                UseListPatternSuggestion { Kind: ListPatternSuggestionKind.FirstOrDefault, DefaultValueArgument: not { } } =>
                    "Replace with 'is [var first, ..] ? first : default'",

                UseListPatternSuggestion { Kind: ListPatternSuggestionKind.LastOrDefault, DefaultValueArgument: { } defaultValueArgument } =>
                    $"Replace with 'is [.., var last] ? last : {defaultValueArgument}'",

                UseListPatternSuggestion { Kind: ListPatternSuggestionKind.LastOrDefault, DefaultValueArgument: not { } } =>
                    "Replace with 'is [.., var last] ? last : default'",

                UseListPatternSuggestion { Kind: ListPatternSuggestionKind.Single } => $"Replace with 'is [var item] ? item : {throwException}'",

                _ => throw new NotSupportedException(),
            };
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

            ModificationUtil.ReplaceChild(
                highlighting.InvocationExpression,
                factory.CreateExpression(
                    highlighting switch
                    {
                        UseIndexerSuggestion { IndexArgument: "0" } => $"$0 is [var first, ..] ? first : {throwThatEmpty}",

                        UseIndexerSuggestion { IndexArgument: "^1" } => $"$0 is [.., var last] ? last : {throwThatEmpty}",

                        UseListPatternSuggestion { Kind: ListPatternSuggestionKind.FirstOrDefault, DefaultValueArgument: { } defaultValueArgument } =>
                            $"$0 is [var first, ..] ? first : {defaultValueArgument}",

                        UseListPatternSuggestion { Kind: ListPatternSuggestionKind.FirstOrDefault, DefaultValueArgument: not { } } =>
                            "$0 is [var first, ..] ? first : default",

                        UseListPatternSuggestion { Kind: ListPatternSuggestionKind.LastOrDefault, DefaultValueArgument: { } defaultValueArgument } =>
                            $"$0 is [.., var last] ? last : {defaultValueArgument}",

                        UseListPatternSuggestion { Kind: ListPatternSuggestionKind.LastOrDefault, DefaultValueArgument: not { } } =>
                            "$0 is [.., var last] ? last : default",

                        UseListPatternSuggestion { Kind: ListPatternSuggestionKind.Single } =>
                            $"$0 is [var item] ? item : {throwThatEmptyOrHasMoreItems}",

                        _ => throw new NotSupportedException(),
                    },
                    highlighting.InvokedExpression.QualifierExpression));
        }

        return _ => { };
    }
}