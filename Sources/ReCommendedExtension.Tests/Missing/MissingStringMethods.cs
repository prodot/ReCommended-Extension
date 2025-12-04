namespace ReCommendedExtension.Tests.Missing;

internal static class MissingStringMethods
{
    extension(string source)
    {
        [Pure]
        public bool Contains(string value, StringComparison comparisonType) => source.IndexOf(value, comparisonType) >= 0;

        [Pure]
        public bool Contains(char value) => source.IndexOf(value) >= 0;

        [Pure]
        public bool Contains(char value, StringComparison comparisonType) => source.IndexOf($"{value}", comparisonType) >= 0;

        [Pure]
        public bool EndsWith(char value) => source.EndsWith($"{value}", StringComparison.Ordinal);

        [Pure]
        public bool StartsWith(char value) => source.StartsWith($"{value}", StringComparison.Ordinal);

        [Pure]
        [ValueRange(-1, int.MaxValue)]
        public int IndexOf(char value, StringComparison comparisonType) => source.IndexOf($"{value}", comparisonType);

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
        public string Replace(string oldValue, string? newValue, StringComparison comparisonType)
        {
            if (comparisonType == StringComparison.Ordinal)
            {
                return source.Replace(oldValue, newValue);
            }

            throw new NotSupportedException();
        }

        [Pure]
        public string[] Split(char separator, StringSplitOptions options = StringSplitOptions.None)
        {
            var result = source.Split([separator], options & ~StringSplitOptions.TrimEntries);

            return (options & StringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
        }

        [Pure]
        public string[] Split(char separator, [NonNegativeValue] int count, StringSplitOptions options = StringSplitOptions.None)
        {
            var result = source.Split([separator], count, options & ~StringSplitOptions.TrimEntries);

            return (options & StringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
        }

        [Pure]
        public string[] Split_(char[]? separator, StringSplitOptions options)
        {
            var result = source.Split(separator, options & ~StringSplitOptions.TrimEntries);

            return (options & StringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
        }

        [Pure]
        public string[] Split_(char[]? separator, [NonNegativeValue] int count, StringSplitOptions options)
        {
            var result = source.Split(separator, count, options & ~StringSplitOptions.TrimEntries);

            return (options & StringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
        }

        [Pure]
        public string[] Split(string? separator, StringSplitOptions options = StringSplitOptions.None)
        {
            var result = source.Split([separator], options & ~StringSplitOptions.TrimEntries);

            return (options & StringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
        }

        [Pure]
        public string[] Split(string? separator, [NonNegativeValue] int count, StringSplitOptions options = StringSplitOptions.None)
        {
            var result = source.Split([separator], count, options & ~StringSplitOptions.TrimEntries);

            return (options & StringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
        }

        [Pure]
        public string[] Split_(string[]? separator, StringSplitOptions options)
        {
            var result = source.Split(separator, options & ~StringSplitOptions.TrimEntries);

            return (options & StringSplitOptions.TrimEntries) != 0 ? [..from item in result select item.Trim()] : result;
        }

        [Pure]
        public string[] Split_(string[]? separator, [NonNegativeValue] int count, StringSplitOptions options)
        {
            var result = source.Split(separator, count, options & ~StringSplitOptions.TrimEntries);

            return (options & StringSplitOptions.TrimEntries) != 0 ? [.. from item in result select item.Trim()] : result;
        }
    }
}