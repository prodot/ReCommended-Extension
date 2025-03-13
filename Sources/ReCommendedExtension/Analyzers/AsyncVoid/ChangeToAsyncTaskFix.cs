using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReCommendedExtension.Analyzers.AsyncVoid;

[QuickFix]
public sealed class ChangeToAsyncTaskFix(AvoidAsyncVoidWarning highlighting) : QuickFixBase
{
    readonly IDeclaredType taskType = highlighting.Declaration.GetPredefinedType().Task;

    public override bool IsAvailable(JetBrains.Util.IUserDataHolder cache) => true;

    public override string Text => $"Change return type to '{taskType.GetClrName().ShortName}'";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            highlighting.Declaration.SetType(taskType.ToIType());
        }

        return _ => { };
    }
}