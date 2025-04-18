using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Extensions;

internal static class TreeNodeCollectionExtensions
{
    /// <remarks>
    /// <paramref name="treeNodeCollection"/> is [var <paramref name="firstItem"/>, .. var <see cref="restItems"/>]
    /// </remarks>
    [Pure]
    public static bool TrySplit<T>(
        this TreeNodeCollection<T> treeNodeCollection,
        [NotNullWhen(true)] out T? firstItem,
        [NotNullWhen(true)] out T[]? restItems) where T : ITreeNode
    {
        if (treeNodeCollection.Count >= 1)
        {
            firstItem = treeNodeCollection[0];
            restItems = treeNodeCollection.Count >= 2 ? treeNodeCollection.Skip(1).ToArray() : [];
            return true;
        }

        firstItem = default;
        restItems = null;
        return false;
    }
}