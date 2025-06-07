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

namespace ReCommendedExtension.Analyzers.Linq;

[QuickFix]
public sealed class RemoveLinqQueryFix(RedundantLinqQueryHint highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => "Remove LINQ query";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.QueryExpression);

            var replacedExpression = ModificationUtil.ReplaceChild(
                highlighting.QueryExpression,
                factory.CreateExpression("$0", highlighting.Expression));

            if (replacedExpression.Parent is ICSharpExpression parent)
            {
                parent.TryRemoveParentheses(factory);
            }
        }

        return _ => { };
    }
}