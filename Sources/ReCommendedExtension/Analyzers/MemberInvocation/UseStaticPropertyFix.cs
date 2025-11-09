using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.MemberInvocation;

[QuickFix]
public sealed class UseStaticPropertyFix(UseStaticPropertySuggestion highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => highlighting.ReferenceExpression.Parent is { };

    public override string Text => $"Replace with '{highlighting.PropertyName}'";

    protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.ReferenceExpression);

            Debug.Assert(highlighting.ReferenceExpression.Parent is { });

            ModificationUtil.ReplaceChild(
                highlighting.ReferenceExpression.Parent,
                highlighting.QualifierExpression is { }
                    ? factory.CreateExpression($"$0.{highlighting.PropertyName}", highlighting.QualifierExpression)
                    : factory.CreateExpression(highlighting.PropertyName));
        }

        return null;
    }
}