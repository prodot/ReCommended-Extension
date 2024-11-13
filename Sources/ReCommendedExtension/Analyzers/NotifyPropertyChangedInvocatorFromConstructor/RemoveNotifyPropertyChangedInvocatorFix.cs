using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

[QuickFix]
public sealed class RemoveNotifyPropertyChangedInvocatorFix(NotifyPropertyChangedInvocatorFromConstructorWarning highlighting) : QuickFixBase
{
    public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache) => true;

    public override string Text => "Remove invocation";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            ModificationUtil.DeleteChildRange(
                highlighting.InvocationExpression,
                highlighting.InvocationExpression.GetNextNonWhitespaceToken() is { } nextToken
                && nextToken.GetTokenType() == CSharpTokenType.SEMICOLON
                    ? nextToken
                    : highlighting.InvocationExpression);
        }

        return _ => { };
    }
}