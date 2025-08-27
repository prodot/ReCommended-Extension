using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Extensions;

internal static class CSharpArgumentsExtensions
{
    [Pure]
    public static bool IsDiscard(this ICSharpArgument argument)
        => argument.Value switch
        {
            IReferenceExpression { NameIdentifier.Name: "_", Reference: var reference } when reference.Resolve().DeclaredElement == null => true,
            IDeclarationExpression { Designation: IDiscardDesignation, IsVar: false } => true,

            _ => false,
        };
}