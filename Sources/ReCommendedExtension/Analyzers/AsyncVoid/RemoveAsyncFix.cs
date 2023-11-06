using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReCommendedExtension.Analyzers.AsyncVoid;

[QuickFix]
public sealed class RemoveAsyncFix : QuickFixBase
{
    readonly AsyncVoidFunctionExpressionWarning highlighting;

    public RemoveAsyncFix(AsyncVoidFunctionExpressionWarning highlighting) => this.highlighting = highlighting;

    public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache) => true;

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            highlighting.RemoveAsyncModifier();
        }

        return _ => { };
    }

    public override string Text => "Remove 'async' modifier";
}