namespace ReCommendedExtension.Tests.Missing;

internal static class MissingStringMethods
{
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
    [ValueRange(-1, int.MaxValue)]
    public static int IndexOf(this string source, char value, StringComparison comparisonType) => source.IndexOf($"{value}", comparisonType);

    [Pure]
    public static string Join(string? separator, params ReadOnlySpan<object?> values) => string.Join(separator, values.ToArray());

    [Pure]
    public static string Join(char separator, params object?[] values) => string.Join($"{separator}", values);

    [Pure]
    public static string Join(char separator, params string?[] values) => string.Join($"{separator}", values);

    [Pure]
    public static string Join(char separator, string?[] values, [NonNegativeValue] int startIndex, [NonNegativeValue] int count)
        => string.Join($"{separator}", values.AsSpan(startIndex, count).ToArray());

    [Pure]
    public static string Join(string? separator, params ReadOnlySpan<string?> values) => string.Join(separator, values.ToArray());

    [Pure]
    public static string Join<T>(char separator, [InstantHandle] IEnumerable<T> values) => string.Join($"{separator}", values);

    [Pure]
    public static string Join(char separator, params ReadOnlySpan<object?> values) => string.Join($"{separator}", values.ToArray());

    [Pure]
    public static string Join(char separator, params ReadOnlySpan<string?> values) => string.Join($"{separator}", values.ToArray());

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
    public static string[] Split(
        this string source,
        char separator,
        [NonNegativeValue] int count,
        MissingStringSplitOptions options = MissingStringSplitOptions.None)
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

        return (options & MissingStringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
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

        return (options & MissingStringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
    }

    [Pure]
    public static string[] Split(
        this string source,
        string[]? separator,
        [NonNegativeValue] int count,
        MissingStringSplitOptions options = MissingStringSplitOptions.None)
    {
        var result = source.Split(separator, count, (StringSplitOptions)(options & ~MissingStringSplitOptions.TrimEntries));

        return (options & MissingStringSplitOptions.TrimEntries) != 0 ? [.. from item in result select item.Trim()] : result;
    }
}