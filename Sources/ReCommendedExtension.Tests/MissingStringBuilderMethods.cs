using System.Text;

namespace ReCommendedExtension.Tests;

internal static class MissingStringBuilderMethods
{
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