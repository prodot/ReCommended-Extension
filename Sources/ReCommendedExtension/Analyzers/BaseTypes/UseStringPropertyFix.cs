using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.BaseTypes;

[QuickFix]
public sealed class UseStringPropertyFix(UseStringPropertySuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Replace with '{highlighting.PropertyName}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

            var conditionalAccess = highlighting.InvokedExpression.HasConditionalAccessSign ? "?" : "";

            ModificationUtil.ReplaceChild(
                highlighting.InvocationExpression,
                factory.CreateExpression($"$0{conditionalAccess}.{highlighting.PropertyName}", highlighting.InvokedExpression.QualifierExpression));
        }

        return _ => { };
    }
}