using System.Globalization;
using System.Text;
using ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingInt128Methods
{
    [Pure]
    public static Int128Analyzer.Int128 Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => Int128Analyzer.Int128.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider);

    [Pure]
    public static Int128Analyzer.Int128 Parse(
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style = NumberStyles.Integer,
        IFormatProvider? provider = null)
        => Int128Analyzer.Int128.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static Int128Analyzer.Int128 Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => Int128Analyzer.Int128.Parse(s.ToString(), NumberStyles.Integer, provider);

    [Pure]
    public static Int128Analyzer.Int128 Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => Int128Analyzer.Int128.Parse(s.ToString(), style, provider);
}