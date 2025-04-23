using System.Globalization;
using System.Text;
using ReCommendedExtension.Analyzers.BaseTypes.Analyzers;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingHalfMethods
{
    [Pure]
    public static HalfAnalyzer.Half Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => HalfAnalyzer.Half.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Float | NumberStyles.AllowThousands, provider);

    [Pure]
    public static HalfAnalyzer.Half Parse(
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands,
        IFormatProvider? provider = null)
        => HalfAnalyzer.Half.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static HalfAnalyzer.Half Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => HalfAnalyzer.Half.Parse(s.ToString(), NumberStyles.Float | NumberStyles.AllowThousands, provider);

    [Pure]
    public static HalfAnalyzer.Half Parse(
        ReadOnlySpan<char> s,
        NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands,
        IFormatProvider? provider = null)
        => HalfAnalyzer.Half.Parse(s.ToString(), style, provider);
}