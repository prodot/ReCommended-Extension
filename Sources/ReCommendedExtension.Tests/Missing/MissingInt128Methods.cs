using System.Globalization;
using System.Text;
using ReCommendedExtension.Extensions.NumberInfos;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingInt128Methods
{
    extension(Int128)
    {
        [Pure]
        public static Int128 Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
            => Int128.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider);

        [Pure]
        public static Int128 Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
            => Int128.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

        [Pure]
        public static Int128 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Int128.Parse(s.ToString(), NumberStyles.Integer, provider);

        [Pure]
        public static Int128 Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
            => Int128.Parse(s.ToString(), style, provider);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Int128 result)
            => Int128.TryParse(s.ToString(), style, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Int128 result)
            => Int128.TryParse(s.ToString(), NumberStyles.Number, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> s, out Int128 result) => Int128.TryParse(s.ToString(), out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out Int128 result)
            => Int128.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out Int128 result)
            => Int128.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Number, provider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<byte> utf8Text, out Int128 result)
            => Int128.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
    }
}