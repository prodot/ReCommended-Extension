using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace ReCommendedExtension.Extensions;

internal static class InvocationExpressionExtensions
{
    [Pure]
    public static bool IsUsedAsStatement(this IInvocationExpression invocationExpression)
        => invocationExpression.Parent is IExpressionStatement or IForInitializer or IForIterator;
}