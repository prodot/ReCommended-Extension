using JetBrains.ReSharper.Psi.CSharp.Parsing;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Extensions;

internal static class CSharpArgumentExtensions
{
    [Pure]
    public static bool IsDiscard(this ICSharpArgument argument)
        => argument.Value switch
        {
            IReferenceExpression { NameIdentifier.Name: "_", Reference: var reference } when reference.Resolve().DeclaredElement == null => true,
            IDeclarationExpression { Designation: IDiscardDesignation, IsVar: false } => true,

            _ => false,
        };

    public static void Remove(this ICSharpArgument argument)
    {
        if (argument.PrevTokens().TakeWhile(t => t.Parent == argument.Parent).FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is
            { } previousCommaToken)
        {
            ModificationUtil.DeleteChildRange(previousCommaToken, argument);
        }
        else
        {
            if (argument.NextTokens().TakeWhile(t => t.Parent == argument.Parent).FirstOrDefault(t => t.GetTokenType() == CSharpTokenType.COMMA) is
                { } nextCommaToken)
            {
                var lastToken =
                    nextCommaToken.NextTokens().TakeWhile(t => t.Parent == argument.Parent).FirstOrDefault(t => !t.IsWhitespaceToken()) is
                        { } nonWhitespaceToken
                        ? nonWhitespaceToken.PrevTokens().First()
                        : nextCommaToken;
                ModificationUtil.DeleteChildRange(argument, lastToken);
            }
            else
            {
                ModificationUtil.DeleteChild(argument);
            }
        }
    }
}