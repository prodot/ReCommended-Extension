using System.Globalization;
using System.Text;
using ReCommendedExtension.Analyzers.BaseTypes.NumberInfos;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingUInt128Methods
{
    [Pure]
    public static UInt128 Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => UInt128.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider);

    [Pure]
    public static UInt128 Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => UInt128.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static UInt128 Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => UInt128.Parse(s.ToString(), NumberStyles.Integer, provider);

    [Pure]
    public static UInt128 Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => UInt128.Parse(s.ToString(), style, provider);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out UInt128 result)
        => UInt128.TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out UInt128 result)
        => UInt128.TryParse(s.ToString(), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out UInt128 result) => UInt128.TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out UInt128 result)
        => UInt128.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out UInt128 result)
        => UInt128.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out UInt128 result)
        => UInt128.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}