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
public sealed class UseSwitchExpressionFix(UseSwitchExpressionSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            const string throwException = $"throw new {nameof(InvalidOperationException)}(...)";

            var replacement = highlighting.DefaultValueArgument is { } defaultValueArgument
                ? $$"""switch { [] => {{defaultValueArgument}}, [var item] => item, _ => {{throwException}} }"""
                : $$"""switch { [] => default, [var item] => item, _ => {{throwException}} }""";

            return $"Replace with '{replacement}'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            const string throwThatHasMoreItems = $"""throw new {nameof(InvalidOperationException)}("List contains more than one element.")""";

            var expression = ModificationUtil.ReplaceChild(
                highlighting.InvocationExpression,
                factory.CreateExpression(
                    highlighting.DefaultValueArgument is { } defaultValueArgument
                        ? $$"""($0 switch { [] => {{defaultValueArgument}}, [var item] => item, _ => {{throwThatHasMoreItems}} })"""
                        : $$"""($0 switch { [] => default, [var item] => item, _ => {{throwThatHasMoreItems}} })""",
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