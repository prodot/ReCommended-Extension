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