using System.Text;

namespace ReCommendedExtension.Tests.Missing;

[SuppressMessage("ReSharper", "UnusedParameter.Global")]
internal static class MissingGuidMethods
{
    extension(Guid)
    {
        [Pure]
        public static Guid Parse(string s, IFormatProvider? provider) => Guid.Parse(s);

        [Pure]
        public static Guid Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Guid.Parse(s.ToString());

        [Pure]
        public static Guid Parse(ReadOnlySpan<char> input) => Guid.Parse(input.ToString());

        [Pure]
        public static Guid Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider) => Guid.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()));

        [Pure]
        public static Guid Parse(ReadOnlySpan<byte> utf8Text) => Guid.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()));

        [Pure]
        public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Guid result) => Guid.TryParse(s, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Guid result) => Guid.TryParse(s.ToString(), out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> input, out Guid result) => Guid.TryParse(input.ToString(), out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out Guid result)
            => TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, out Guid result)
            => TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
    }
}