using System.Collections;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace ReCommendedExtension.Extensions;

internal static class TreeNodeCollectionExtensions
{
    sealed class ReadOnlyListView<T>(
        TreeNodeCollection<T> treeNodeCollection,
        [NonNegativeValue] int indexOffset,
        [ValueRange(1, int.MaxValue)] int length) : IReadOnlyList<T> where T : ITreeNode
    {
        public T this[int index]
        {
            get
            {
                Debug.Assert(index >= 0);
                Debug.Assert(index < length);

                return treeNodeCollection[index + indexOffset];
            }
        }

        public int Count => length;

        [MustDisposeResource]
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = indexOffset; i < indexOffset + length; i++)
            {
                yield return treeNodeCollection[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

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

    /// <returns>The <paramref name="arguments"/> if it doesn't contain any optional argument.</returns>
    [Pure]
    public static TreeNodeCollection<ICSharpArgument>? AsAllNonOptionalOrNull(this TreeNodeCollection<ICSharpArgument?> arguments)
        => arguments.Contains(null) ? null : arguments!;

    [Pure]
    public static IReadOnlyList<T> GetSubrange<T>(this TreeNodeCollection<T> treeNodeCollection, Range range) where T : ITreeNode
    {
        var (indexOffset, length) = range.GetOffsetAndLength(treeNodeCollection.Count);

        if (length == 0)
        {
            return [];
        }

        return new ReadOnlyListView<T>(treeNodeCollection, indexOffset, length);
    }
}