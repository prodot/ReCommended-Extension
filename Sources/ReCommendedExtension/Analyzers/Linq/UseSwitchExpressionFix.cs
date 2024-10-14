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
public sealed class UseSwitchExpressionFix(UseSwitchExpressionSuggestion highlighting) : QuickFixBase
{
    string GetReplacement(string? multipleItemsMessage = null)
    {
        var throwThatHasMoreItems =
            $"throw new {nameof(InvalidOperationException)}({(multipleItemsMessage is { } ? $"\"{multipleItemsMessage}\"" : "...")})";

        return highlighting.DefaultValueArgument is { } defaultValueArgument
            ? $$"""switch { [] => {{defaultValueArgument}}, [var item] => item, _ => {{throwThatHasMoreItems}} }"""
            : $$"""switch { [] => default, [var item] => item, _ => {{throwThatHasMoreItems}} }""";
    }

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{GetReplacement()}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var replacement = GetReplacement("List contains more than one element.");

            ModificationUtil
                .ReplaceChild(
                    highlighting.InvocationExpression,
                    factory.CreateExpression($"($0 {replacement})", highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}