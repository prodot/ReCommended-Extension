using System.Collections;

namespace ReCommendedExtension.Extensions;

internal static class LinqExtensions
{
    sealed class ReadOnlyListView<T>(IReadOnlyList<T> list, [NonNegativeValue] int ignoreIndex) : IReadOnlyList<T>
    {
        public T this[int index] => index < ignoreIndex ? list[index] : list[index + 1];

        public int Count => list.Count - 1;

        [MustDisposeResource]
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (i != ignoreIndex)
                {
                    yield return list[i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Pure]
    public static IReadOnlyList<T> WithoutElementAt<T>(this IReadOnlyList<T> list, [NonNegativeValue] int ignoreIndex)
    {
        if (ignoreIndex == 0 && list.Count == 1)
        {
            return [];
        }

        Debug.Assert(ignoreIndex < list.Count);

        return new ReadOnlyListView<T>(list, ignoreIndex);
    }
}