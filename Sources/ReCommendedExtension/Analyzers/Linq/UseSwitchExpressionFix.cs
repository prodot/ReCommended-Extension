using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Linq;

[QuickFix]
public sealed class UseSwitchExpressionFix(UseSwitchExpressionSuggestion highlighting) : QuickFixBase
{
    string GetReplacement(string? exceptionMessage = null)
    {
        var throwExpression =
            $"throw new {nameof(InvalidOperationException)}({(exceptionMessage is { } ? $"\"{exceptionMessage}\"" : "...")})";

        return highlighting.DefaultValueExpression is { }
            ? $$"""switch { [] => {{highlighting.DefaultValueExpression}}, [var item] => item, _ => {{throwExpression}} }"""
            : $$"""switch { [] => default, [var item] => item, _ => {{throwExpression}} }""";
    }

    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{GetReplacement()}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            Debug.Assert(highlighting.InvokedExpression.QualifierExpression is { });

            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var replacement = highlighting.InvokedExpression.QualifierExpression.Type().IsString()
                ? GetReplacement("String contains more than one character.")
                : GetReplacement("List contains more than one element.");

            ModificationUtil
                .ReplaceChild(
                    highlighting.InvocationExpression,
                    factory.CreateExpression($"($0 {replacement})", highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveParentheses(factory);
        }

        return _ => { };
    }
}