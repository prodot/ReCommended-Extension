namespace ReCommendedExtension.Tests.Missing;

internal static class MissingEnumerableMethods
{
    extension<T>([InstantHandle] IEnumerable<T> source)
    {
        [Pure]
        public T ElementAt(Index index)
            => index.IsFromEnd ? Enumerable.ElementAt(source.Reverse(), index.Value - 1) : Enumerable.ElementAt(source, index.Value);

        [Pure]
        public T FirstOrDefault(T defaultValue)
        {
            foreach (var item in source)
            {
                return item;
            }

            return defaultValue;
        }

        [Pure]
        public T LastOrDefault(T defaultValue)
        {
            var (lastItem, lastItemFound) = (default(T)!, false);

            foreach (var item in source)
            {
                (lastItem, lastItemFound) = (item, true);
            }

            return lastItemFound ? lastItem : defaultValue;
        }
    }
}