using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Strings;

[QuickFix]
public sealed class RemoveArgumentFix(RedundantArgumentHint highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => "Remove argument";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            if (highlighting
                    .Argument.PrevTokens()
                    .TakeWhile(t => t.Parent == highlighting.Argument.Parent)
                    .FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is { } commaToken)
            {
                ModificationUtil.DeleteChildRange(commaToken, highlighting.Argument);
            }
            else
            {
                ModificationUtil.DeleteChild(highlighting.Argument);
            }
        }

        return _ => { };
    }
}