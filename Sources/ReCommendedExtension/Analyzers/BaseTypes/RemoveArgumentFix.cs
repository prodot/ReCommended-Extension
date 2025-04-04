using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.BaseTypes;

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
                    .FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is { } previousCommaToken)
            {
                ModificationUtil.DeleteChildRange(previousCommaToken, highlighting.Argument);
            }
            else
            {
                if (highlighting
                        .Argument.NextTokens()
                        .TakeWhile(t => t.Parent == highlighting.Argument.Parent)
                        .FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is { } nextCommaToken)
                {
                    var lastToken = nextCommaToken
                        .NextTokens()
                        .TakeWhile(t => t.Parent == highlighting.Argument.Parent)
                        .FirstOrDefault(t => !t.IsWhitespaceToken()) is { } nonWhitespaceToken
                        ? nonWhitespaceToken.PrevTokens().First()
                        : nextCommaToken;
                    ModificationUtil.DeleteChildRange(highlighting.Argument, lastToken);
                }
                else
                {
                    ModificationUtil.DeleteChild(highlighting.Argument);
                }
            }
        }

        return _ => { };
    }
}