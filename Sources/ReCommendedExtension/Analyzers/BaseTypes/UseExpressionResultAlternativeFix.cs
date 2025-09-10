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

namespace ReCommendedExtension.Analyzers.BaseTypes;

[QuickFix]
public sealed class UseExpressionResultAlternativeFix(UseExpressionResultSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => highlighting.AlternativeReplacement is { };

    public override string Text
    {
        get
        {
            Debug.Assert(highlighting.AlternativeReplacement is { });

            return $"Replace with '{highlighting.AlternativeReplacement.TrimToSingleLineWithMaxLength(120)}'";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.Expression);

            var expression = ModificationUtil
                .ReplaceChild(highlighting.Expression, factory.CreateExpression($"({highlighting.AlternativeReplacement})"))
                .TryRemoveParentheses(factory);

            if (expression is IUnaryOperatorExpression unaryOperatorExpression)
            {
                unaryOperatorExpression.Operand.TryRemoveParentheses(factory);
            }
        }

        return _ => { };
    }
}