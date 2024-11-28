using System.Text;

namespace ReCommendedExtension.Tests;

internal static class MissingMethods
{
    [Pure]
    public static T FirstOrDefault<T>(this IEnumerable<T> source, T defaultValue)
    {
        foreach (var item in source)
        {
            return item;
        }

        return defaultValue;
    }

    [Pure]
    public static T LastOrDefault<T>(this IEnumerable<T> source, T defaultValue)
    {
        var (lastItem, lastItemFound) = (default(T)!, false);

        foreach (var item in source)
        {
            (lastItem, lastItemFound) = (item, true);
        }

        return lastItemFound ? lastItem : defaultValue;
    }

    [Pure]
    public static bool Contains(this string source, string value, StringComparison comparisonType) => source.IndexOf(value, comparisonType) >= 0;

    [Pure]
    public static bool Contains(this string source, char value) => source.IndexOf(value) >= 0;

    [Pure]
    public static bool Contains(this string source, char value, StringComparison comparisonType) => source.IndexOf($"{value}", comparisonType) >= 0;

    [Pure]
    public static bool EndsWith(this string source, char value) => source.EndsWith($"{value}", StringComparison.Ordinal);

    [Pure]
    public static bool StartsWith(this string source, char value) => source.StartsWith($"{value}", StringComparison.Ordinal);

    [Pure]
    public static int IndexOf(this string source, char value, StringComparison comparisonType) => source.IndexOf($"{value}", comparisonType);

    [Pure]
    public static string Replace(this string source, string oldValue, string? newValue, StringComparison comparisonType)
    {
        if (comparisonType == StringComparison.Ordinal)
        {
            return source.Replace(oldValue, newValue);
        }

        throw new NotSupportedException();
    }

    [Pure]
    public static string[] Split(this string source, char separator, MissingStringSplitOptions options = MissingStringSplitOptions.None)
    {
        var result = source.Split([separator], (StringSplitOptions)(options & ~MissingStringSplitOptions.TrimEntries));

        return (options & MissingStringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
    }

    [Pure]
    public static string[] Split(this string source, char separator, int count, MissingStringSplitOptions options = MissingStringSplitOptions.None)
    {
        var result = source.Split([separator], count, (StringSplitOptions)(options & ~MissingStringSplitOptions.TrimEntries));

        return (options & MissingStringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
    }

    [Pure]
    public static string[] Split(this string source, char[]? separator, MissingStringSplitOptions options)
    {
        var result = source.Split(separator, (StringSplitOptions)(options & ~MissingStringSplitOptions.TrimEntries));

        return (options & MissingStringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
    }

    [Pure]
    public static string[] Split(
        this string source,
        char[]? separator,
        [NonNegativeValue] int count,
        MissingStringSplitOptions options = MissingStringSplitOptions.None)
    {
        var result = source.Split(separator, count, (StringSplitOptions)(options & ~MissingStringSplitOptions.TrimEntries));

        return (options & MissingStringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
    }

    [Pure]
    public static string[] Split(this string source, string? separator, MissingStringSplitOptions options = MissingStringSplitOptions.None)
    {
        var result = source.Split([separator], (StringSplitOptions)(options & ~MissingStringSplitOptions.TrimEntries));

        return (options & MissingStringSplitOptions.TrimEntries) != 0 ? [.. from item in result select item.Trim()] : result;
    }

    [Pure]
    public static string[] Split(
        this string source,
        string? separator,
        [NonNegativeValue] int count,
        MissingStringSplitOptions options = MissingStringSplitOptions.None)
    {
        var result = source.Split([separator], count, (StringSplitOptions)(options & ~MissingStringSplitOptions.TrimEntries));

        return (options & MissingStringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
    }

    [Pure]
    public static string[] Split(this string source, string[]? separator, MissingStringSplitOptions options = MissingStringSplitOptions.None)
    {
        var result = source.Split(separator, (StringSplitOptions)(options & ~MissingStringSplitOptions.TrimEntries));

        return (options & MissingStringSplitOptions.TrimEntries) != 0 ? [.. from item in result select item.Trim()] : result;
    }

    [Pure]
    public static string[] Split(
        this string source,
        string[]? separator,
        int count,
        MissingStringSplitOptions options = MissingStringSplitOptions.None)
    {
        var result = source.Split(separator, count, (StringSplitOptions)(options & ~MissingStringSplitOptions.TrimEntries));

        return (options & MissingStringSplitOptions.TrimEntries) != 0 ? [.. from item in result select item.Trim()] : result;
    }

    public static StringBuilder Append(
        this StringBuilder source,
        StringBuilder? value,
        [NonNegativeValue] int startIndex,
        [NonNegativeValue] int count)
        => (value, startIndex, count) switch
        {
            (not { }, 0, 0) or ({ }, _, 0) => source,
            _ => source.Append(value?.ToString(startIndex, count)),
        };

    public static StringBuilder AppendJoin(this StringBuilder source, string? separator, params object?[] values)
        => source.Append(string.Join(separator, values));

    public static StringBuilder AppendJoin(this StringBuilder source, string? separator, params ReadOnlySpan<object?> values)
        => source.Append(string.Join(separator, values.ToArray()));

    public static StringBuilder AppendJoin<T>(this StringBuilder source, string? separator, [InstantHandle] IEnumerable<T> values)
        => source.Append(string.Join(separator, values));

    public static StringBuilder AppendJoin(this StringBuilder source, string? separator, params string?[] values)
        => source.Append(string.Join(separator, values));

    public static StringBuilder AppendJoin(this StringBuilder source, string? separator, params ReadOnlySpan<string?> values)
        => source.Append(string.Join(separator, values.ToArray()));

    public static StringBuilder AppendJoin(this StringBuilder source, char separator, params object?[] values)
        => source.Append(string.Join($"{separator}", values));

    public static StringBuilder AppendJoin(this StringBuilder source, char separator, params ReadOnlySpan<object?> values)
        => source.Append(string.Join($"{separator}", values.ToArray()));

    public static StringBuilder AppendJoin<T>(this StringBuilder source, char separator, [InstantHandle] IEnumerable<T> values)
        => source.Append(string.Join($"{separator}", values));

    public static StringBuilder AppendJoin(this StringBuilder source, char separator, params string?[] values)
        => source.Append(string.Join($"{separator}", values));

    public static StringBuilder AppendJoin(this StringBuilder source, char separator, params ReadOnlySpan<string?> values)
        => source.Append(string.Join($"{separator}", values.ToArray()));
}