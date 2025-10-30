using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[QuickFix]
public sealed class UsePatternFix(UsePatternSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            var expression = highlighting.Replacement.Expression.GetText().TrimToSingleLineWithMaxLength(120);
            var pattern = highlighting.Replacement.Pattern.TrimToSingleLineWithMaxLength(120);

            return $"Replace with '{expression} is {pattern}'";
        }
    }

    protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var expression = ModificationUtil
                .ReplaceChild(
                    highlighting.InvocationExpression,
                    factory.CreateExpression($"(($0) is {highlighting.Replacement.Pattern})", highlighting.Replacement.Expression))
                .TryRemoveParentheses(factory);

            if (expression is IIsExpression isExpression)
            {
                isExpression.Operand.TryRemoveParentheses(factory);
            }
        }

        return _ => { };
    }
}