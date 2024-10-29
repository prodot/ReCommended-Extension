using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.DelegateInvoke;

[QuickFix]
public sealed class RemoveDelegateInvokeFix(RedundantDelegateInvokeHint highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => $"Remove '{nameof(Action.Invoke)}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var dotToken = highlighting.ReferenceExpression.NameIdentifier.GetPreviousMeaningfulToken();
            Debug.Assert(dotToken is { });

            ModificationUtil.DeleteChildRange(dotToken, highlighting.ReferenceExpression.NameIdentifier);
        }

        return _ => { };
    }
}