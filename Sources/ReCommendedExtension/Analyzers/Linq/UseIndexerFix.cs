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

    public override string Text
    {
        get
        {
            var otherException = highlighting.CanThrowOtherException ? " (other exception could be thrown)" : "";

            return $"Replace with '[{highlighting.IndexArgument.TrimToSingleLineWithMaxLength(120)}]'{otherException}";
        }
    }

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var conditionalAccess = highlighting.InvokedExpression.HasConditionalAccessSign ? "?" : "";

            ModificationUtil.ReplaceChild(
                highlighting.InvocationExpression,
                factory.CreateExpression($"$0{conditionalAccess}[{highlighting.IndexArgument}]", highlighting.InvokedExpression.QualifierExpression));
        }

        return _ => { };
    }
}