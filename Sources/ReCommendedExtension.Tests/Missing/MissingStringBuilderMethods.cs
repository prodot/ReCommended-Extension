using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingStringBuilderMethods
{
    extension(StringBuilder source)
    {
        public StringBuilder Append(StringBuilder? value, [NonNegativeValue] int startIndex, [NonNegativeValue] int count)
            => (value, startIndex, count) switch
            {
                (null, 0, 0) or ({ }, _, 0) => source,
                _ => source.Append(value?.ToString(startIndex, count)),
            };

        public StringBuilder AppendJoin(string? separator, params object?[] values) => source.Append(string.Join(separator, values));

        public StringBuilder AppendJoin(string? separator, params ReadOnlySpan<object?> values)
            => source.Append(string.Join(separator, values.ToArray()));

        public StringBuilder AppendJoin<T>(string? separator, [InstantHandle] IEnumerable<T> values) => source.Append(string.Join(separator, values));

        public StringBuilder AppendJoin(string? separator, params string?[] values) => source.Append(string.Join(separator, values));

        public StringBuilder AppendJoin(string? separator, params ReadOnlySpan<string?> values)
            => source.Append(string.Join(separator, values.ToArray()));

        public StringBuilder AppendJoin(char separator, params object?[] values) => source.Append(string.Join($"{separator}", values));

        public StringBuilder AppendJoin(char separator, params ReadOnlySpan<object?> values)
            => source.Append(string.Join($"{separator}", values.ToArray()));

        public StringBuilder AppendJoin<T>(char separator, [InstantHandle] IEnumerable<T> values)
            => source.Append(string.Join($"{separator}", values));

        public StringBuilder AppendJoin(char separator, params string?[] values) => source.Append(string.Join($"{separator}", values));

        public StringBuilder AppendJoin(char separator, params ReadOnlySpan<string?> values)
            => source.Append(string.Join($"{separator}", values.ToArray()));
    }
}