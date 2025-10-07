using JetBrains.Application.Progress;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Feature.Services.QuickFixes;
using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;
using JetBrains.Util;

namespace ReCommendedExtension.Analyzers.Argument;

[QuickFix]
public sealed class RemoveArgumentRangeFix(RedundantArgumentRangeHint highlighting) : QuickFixBase
{
    public override bool IsAvailable(IUserDataHolder cache) => true;

    public override string Text => "Remove arguments";

    protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
    {
        using (WriteLockCookie.Create())
        {
            var firstArgument = highlighting.Arguments[0];
            var lastArgument = highlighting.Arguments[^1];

            if (firstArgument
                    .PrevTokens()
                    .TakeWhile(t => t.Parent == firstArgument.Parent)
                    .FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is { } previousCommaToken)
            {
                ModificationUtil.DeleteChildRange(previousCommaToken, lastArgument);
            }
            else
            {
                if (lastArgument
                        .NextTokens()
                        .TakeWhile(t => t.Parent == lastArgument.Parent)
                        .FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is { } nextCommaToken)
                {
                    var lastToken = nextCommaToken
                        .NextTokens()
                        .TakeWhile(t => t.Parent == lastArgument.Parent)
                        .FirstOrDefault(t => !t.IsWhitespaceToken()) is { } nonWhitespaceToken
                        ? nonWhitespaceToken.PrevTokens().First()
                        : nextCommaToken;
                    ModificationUtil.DeleteChildRange(firstArgument, lastToken);
                }
                else
                {
                    ModificationUtil.DeleteChildRange(firstArgument, lastArgument);
                }
            }
        }

        return _ => { };
    }
}