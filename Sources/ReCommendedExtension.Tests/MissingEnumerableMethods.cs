namespace ReCommendedExtension.Tests;

internal static class MissingEnumerableMethods
{
    [Pure]
    public static T FirstOrDefault<T>([InstantHandle] this IEnumerable<T> source, T defaultValue)
    {
        foreach (var item in source)
        {
            return item;
        }

        return defaultValue;
    }

    [Pure]
    public static T LastOrDefault<T>([InstantHandle] this IEnumerable<T> source, T defaultValue)
    {
        var (lastItem, lastItemFound) = (default(T)!, false);

        foreach (var item in source)
        {
            (lastItem, lastItemFound) = (item, true);
        }

        return lastItemFound ? lastItem : defaultValue;
    }
}