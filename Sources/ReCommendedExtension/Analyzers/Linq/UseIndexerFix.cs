using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Linq;

[QuickFix]
public sealed class UseIndexerFix(UseIndexerSuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '[{highlighting.IndexArgument}]' (other exception could be thrown)";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            ModificationUtil.ReplaceChild(
                highlighting.InvocationExpression,
                factory.CreateExpression($"$0[{highlighting.IndexArgument}]", highlighting.InvokedExpression.QualifierExpression));
        }

        return _ => { };
    }
}