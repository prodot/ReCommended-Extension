using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Extensions;

internal static class TreeNodeCollectionExtensions
{
    /// <remarks>
    /// <paramref name="treeNodeCollection"/> is [var <paramref name="firstItem"/>, .. var <see cref="restItems"/>]
    /// </remarks>
    [Pure]
    public static bool TrySplit<T>(
        this TreeNodeCollection<T?> treeNodeCollection,
        [NotNullWhen(true)] out T? firstItem,
        [NotNullWhen(true)] out T[]? restItems) where T : ITreeNode
    {
        if (treeNodeCollection.Count >= 1 && treeNodeCollection.All(item => item is { }))
        {
            var collection = (TreeNodeCollection<T>)treeNodeCollection!;

            firstItem = collection[0];
            restItems = collection.Count >= 2 ? collection.Skip(1).ToArray() : [];

            return true;
        }

        firstItem = default;
        restItems = null;
        return false;
    }
}