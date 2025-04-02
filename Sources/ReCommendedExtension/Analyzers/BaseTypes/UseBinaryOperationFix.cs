using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;
using ReCommendedExtension.Extensions;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[QuickFix]
public sealed class UseBinaryOperationFix(UseBinaryOperationSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text
    {
        get
        {
            var leftOperand = highlighting.LeftOperand.GetText().TrimToSingleLineWithMaxLength(120);
            var rightOperand = highlighting.RightOperand.GetText().TrimToSingleLineWithMaxLength(120);

            return $"Replace with '{leftOperand} {highlighting.Operator} {rightOperand}'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var leftOperand = $"({highlighting.LeftOperand.GetText()})";
            var rightOperand = $"({highlighting.RightOperand.GetText()})";

            ModificationUtil
                .ReplaceChild(
                    highlighting.InvocationExpression,
                    factory.CreateExpression($"{leftOperand} {highlighting.Operator} {rightOperand}"))
                .TryRemoveBinaryOperatorParentheses(factory);
        }

        return _ => { };
    }
}