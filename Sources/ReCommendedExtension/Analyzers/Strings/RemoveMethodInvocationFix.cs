using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Strings;

[QuickFix]
public sealed class RemoveMethodInvocationFix(RedundantMethodInvocationHint highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => "Remove method invocation";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            if (highlighting.RemoveEntireInvocationExpression())
            {
                ModificationUtil.DeleteChildRange(
                    highlighting.InvocationExpression,
                    highlighting.InvocationExpression.GetNextNonWhitespaceToken() is { } nextToken
                    && nextToken.GetTokenType() == CSharpTokenType.SEMICOLON
                        ? nextToken
                        : highlighting.InvocationExpression);
            }
            else
            {
                var factory = CSharpElementFactory.GetInstance(highlighting.InvocationExpression);

                ModificationUtil
                    .ReplaceChild(
                        highlighting.InvocationExpression,
                        factory.CreateExpression("($0)", highlighting.InvokedExpression.QualifierExpression))
                    .TryRemoveParentheses(factory);
            }
        }

        return _ => { };
    }
}