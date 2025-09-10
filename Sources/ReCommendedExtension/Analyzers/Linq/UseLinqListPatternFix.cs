using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.Linq;

[QuickFix]
public sealed class UseLinqListPatternFix(UseLinqListPatternSuggestion highlighting) : QuickFixBase
{
    [Pure]
    string GetReplacement(string? exceptionMessage = null)
    {
        var throwExpression =
            $"throw new {nameof(InvalidOperationException)}({(exceptionMessage is { } ? $"\"{exceptionMessage}\"" : "...")})";

        return highlighting.Kind switch
        {
            ListPatternSuggestionKind.FirstOrDefault => highlighting.DefaultValueExpression is { }
                ? $"is [var first, ..] ? first : {highlighting.DefaultValueExpression}"
                : "is [var first, ..] ? first : default",

            ListPatternSuggestionKind.LastOrDefault => highlighting.DefaultValueExpression is { }
                ? $"is [.., var last] ? last : {highlighting.DefaultValueExpression}"
                : "is [.., var last] ? last : default",

            ListPatternSuggestionKind.Single => $"is [var item] ? item : {throwExpression}",

            _ => throw new NotSupportedException(),
        };
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
                ? GetReplacement("String is either empty or contains more than one character.")
                : GetReplacement("List is either empty or contains more than one element.");

            var expression = ModificationUtil
                .ReplaceChild(
                    highlighting.InvocationExpression,
                    factory.CreateExpression($"(($0) {replacement})", highlighting.InvokedExpression.QualifierExpression))
                .TryRemoveParentheses(factory);

            if (expression is IConditionalTernaryExpression { ConditionOperand: IIsExpression isExpression })
            {
                isExpression.Operand.TryRemoveParentheses(factory);
            }
        }

        return _ => { };
    }
}