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

namespace ReCommendedExtension.Analyzers.ExpressionResult;

[QuickFix]
public sealed class UseExpressionResultFix(UseExpressionResultSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{highlighting.Replacements.Main.TrimToSingleLineWithMaxLength(120)}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.Expression);

            var expression = ModificationUtil
                .ReplaceChild(highlighting.Expression, factory.CreateExpression($"({highlighting.Replacements.Main})"))
                .TryRemoveParentheses(factory);

            if (expression is IUnaryOperatorExpression unaryOperatorExpression)
            {
                unaryOperatorExpression.Operand.TryRemoveParentheses(factory);
            }
        }

        return _ => { };
    }
}