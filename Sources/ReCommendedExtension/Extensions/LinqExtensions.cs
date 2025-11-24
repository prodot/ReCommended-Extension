using System.Collections;

namespace ReCommendedExtension.Extensions;

internal static class LinqExtensions
{
    sealed class ReadOnlyListView<T>(
        IReadOnlyList<T> list,
        [NonNegativeValue] int ignoreIndexOffset,
        [ValueRange(1, int.MaxValue)] int ignoreLength = 1) : IReadOnlyList<T>
    {
        public T this[int index] => index < ignoreIndexOffset ? list[index] : list[index + ignoreLength];

        public int Count => list.Count - ignoreLength;

        [MustDisposeResource]
        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (i < ignoreIndexOffset || i >= ignoreIndexOffset + ignoreLength)
                {
                    yield return list[i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    extension<T>(IReadOnlyList<T> list)
    {
        [Pure]
        public IReadOnlyList<T> WithoutElementAt([NonNegativeValue] int ignoreIndex)
        {
            if (ignoreIndex == 0 && list.Count == 1)
            {
                return [];
            }

            Debug.Assert(ignoreIndex < list.Count);

            return new ReadOnlyListView<T>(list, ignoreIndex);
        }

        [Pure]
        public IReadOnlyList<T> WithoutElementsAt(Range ignoreRange)
        {
            var (ignoreIndexOffset, ignoreLength) = ignoreRange.GetOffsetAndLength(list.Count);

            if (ignoreLength == list.Count)
            {
                return [];
            }

            return new ReadOnlyListView<T>(list, ignoreIndexOffset, ignoreLength);
        }
    }
}