using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.ValueTask;

[QuickFix]
public sealed class InsertAsTaskFix(IntentionalBlockingAttemptWarning highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => "Insert '.AsTask()'";

    protected override Action<ITextControl>? ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var factory = CSharpElementFactory.GetInstance(highlighting.Expression);

            ModificationUtil.ReplaceChild(
                highlighting.Expression,
                factory.CreateExpression("$0.AsTask().GetAwaiter().GetResult", highlighting.ValueTaskExpression));
        }

        return null;
    }
}