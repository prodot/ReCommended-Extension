using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Extensions;

internal static class CSharpInvocationInfoExtensions
{
    [Pure]
    public static bool IsUsedAsStatement(this ICSharpInvocationInfo invocationInfo)
        => invocationInfo is ICSharpTreeNode { Parent: IExpressionStatement or IForInitializer or IForIterator };
}