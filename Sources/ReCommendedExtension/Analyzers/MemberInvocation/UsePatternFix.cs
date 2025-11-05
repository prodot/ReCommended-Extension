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
            var pattern = (highlighting.Replacement.PatternDisplayText ?? highlighting.Replacement.Pattern).TrimToSingleLineWithMaxLength(120);

            if (highlighting.Replacement.HighlightOnlyInvokedMethod)
            {
                return $"Replace with '{pattern}'";
            }

            var expression = highlighting.Replacement.Expression.GetText().TrimToSingleLineWithMaxLength(120);

            return $"Replace with '{expression} {pattern}'";
        }
    }

    protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.Expression);

            var expression = ModificationUtil
                .ReplaceChild(
                    highlighting.Expression,
                    factory.CreateExpression($"(($0) {highlighting.Replacement.Pattern})", highlighting.Replacement.Expression))
                .TryRemoveParentheses(factory);

            switch (expression)
            {
                case IIsExpression isExpression: isExpression.Operand.TryRemoveParentheses(factory); break;

                case IBinaryExpression binaryExpression:
                    (binaryExpression.LeftOperand as IIsExpression)?.Operand.TryRemoveParentheses(factory);
                    (binaryExpression.RightOperand as IBinaryExpression)?.RightOperand.TryRemoveParentheses(factory);
                    break;

                case IConditionalTernaryExpression conditionalTernaryExpression:
                    (conditionalTernaryExpression.ConditionOperand as IIsExpression)?.Operand.TryRemoveParentheses(factory);
                    conditionalTernaryExpression.ElseResult.TryRemoveParentheses(factory);
                    break;

                case ISwitchExpression switchExpression:
                    switchExpression.GoverningExpression.TryRemoveParentheses(factory);
                    if (switchExpression.Arms is [var arm, ..])
                    {
                        arm.Expression.TryRemoveParentheses(factory);
                    }
                    break;
            }
        }

        return null;
    }
}