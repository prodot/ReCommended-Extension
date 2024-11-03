using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Strings;

[QuickFix]
public sealed class UseStringListPatternFix(UseStringListPatternSuggestion highlighting) : QuickFixBase
{
    string GetReplacement(bool forUI = false)
    {
        switch (highlighting)
        {
            case { Characters: { } characters }:
                var pattern = !forUI || characters.All(c => c.IsPrintable())
                    ? string.Join(" or ", from c in characters select $"'{c}'")
                    : "<pattern>";

                return highlighting.Kind switch
                {
                    ListPatternSuggestionKind.FirstCharacter => $"is [{pattern}, ..]",
                    ListPatternSuggestionKind.NotFirstCharacter => $"is not [{pattern}, ..]",
                    ListPatternSuggestionKind.LastCharacter => $"is [.., {pattern}]",
                    ListPatternSuggestionKind.NotLastCharacter => $"is not [.., {pattern}]",

                    _ => throw new NotSupportedException(),
                };

            case { ValueArgument: { } valueArgument }:
                if (forUI)
                {
                    valueArgument = valueArgument.TrimToSingleLineWithMaxLength(120);
                }

                return highlighting.Kind switch
                {
                    ListPatternSuggestionKind.FirstCharacter => $"is [var firstCharacter, ..] && firstCharacter == {valueArgument}",
                    ListPatternSuggestionKind.NotFirstCharacter => $"is not [var firstCharacter, ..] || firstCharacter != {valueArgument}",
                    ListPatternSuggestionKind.LastCharacter => $"is [.., var lastCharacter] && lastCharacter == {valueArgument}",
                    ListPatternSuggestionKind.NotLastCharacter => $"is not [.., var lastCharacter] || lastCharacter != {valueArgument}",

                    _ => throw new NotSupportedException(),
                };

            default: throw new NotSupportedException();
        }
    }

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{GetReplacement(true)}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            ModificationUtil
                .ReplaceChild(
                    highlighting.BinaryExpression as ITreeNode ?? highlighting.InvocationExpression,
                    factory.CreateExpression($"($0 {GetReplacement()})", highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}