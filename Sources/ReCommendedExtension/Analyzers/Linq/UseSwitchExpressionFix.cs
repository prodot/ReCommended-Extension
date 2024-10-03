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
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            const string throwException = $"throw new {nameof(InvalidOperationException)}(...)";

            return highlighting.DefaultValueArgument is { } defaultValueArgument
                ? $$"""Replace with 'switch { [] => {{defaultValueArgument}}, [var item] => item, _ => {{throwException}} }'"""
                : $$"""Replace with 'switch { [] => default, [var item] => item, _ => {{throwException}} }'""";
        }
    }

    protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            const string throwThatHasMoreItems = $"""throw new {nameof(InvalidOperationException)}("List contains more than one element.")""";

            ModificationUtil.ReplaceChild(
                highlighting.InvocationExpression,
                factory.CreateExpression(
                    highlighting.DefaultValueArgument is { } defaultValueArgument
                        ? $$"""$0 switch { [] => {{defaultValueArgument}}, [var item] => item, _ => {{throwThatHasMoreItems}} }"""
                        : $$"""$0 switch { [] => default, [var item] => item, _ => {{throwThatHasMoreItems}} }""",
                    highlighting.InvokedExpression.QualifierExpression));
        }

        return _ => { };
    }
}