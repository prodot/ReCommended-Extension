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
public sealed class UseExpressionResultFix(UseExpressionResultSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{highlighting.Replacement.TrimToSingleLineWithMaxLength(120)}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            ModificationUtil
                .ReplaceChild(highlighting.InvocationExpression, factory.CreateExpression($"{highlighting.Replacement}"))
                .TryRemoveUnaryOperatorParentheses(factory);
        }

        return _ => { };
    }
}