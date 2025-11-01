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
public sealed class UseRangeIndexerFix(UseRangeIndexerSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '[{highlighting.Replacement.IndexDisplayText.TrimToSingleLineWithMaxLength(120)}]'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var conditionalAccess = highlighting.InvokedExpression.HasConditionalAccessSign ? "?" : "";

            var expression = ModificationUtil.ReplaceChild(
                highlighting.InvocationExpression,
                factory.CreateExpression(
                    $"$0{conditionalAccess}[{highlighting.Replacement.Index}]",
                    highlighting.InvokedExpression.QualifierExpression));

            if (expression is IElementAccessExpression { Arguments: [{ Value: IRangeExpression rangeExpression }] })
            {
                rangeExpression.LeftOperand?.TryRemoveParentheses(factory);
                rangeExpression.RightOperand?.TryRemoveParentheses(factory);
            }
        }

        return _ => { };
    }
}