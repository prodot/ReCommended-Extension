using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Extensions;

internal static class CSharpInvocationInfoExtensions
{
    extension(ICSharpInvocationInfo invocationInfo)
    {
        public bool IsUsedAsStatement => invocationInfo is ICSharpTreeNode { Parent: IExpressionStatement or IForInitializer or IForIterator };
    }
}