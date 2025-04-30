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

    [Pure]
    public static HalfAnalyzer.Half Round(HalfAnalyzer.Half x) => (HalfAnalyzer.Half)MissingSingleMethods.Round((float)x);

    [Pure]
    public static HalfAnalyzer.Half Round(HalfAnalyzer.Half x, [ValueRange(0, 6)] int digits)
        => (HalfAnalyzer.Half)MissingSingleMethods.Round((float)x, digits);

    [Pure]
    public static HalfAnalyzer.Half Round(HalfAnalyzer.Half x, MidpointRounding mode)
        => (HalfAnalyzer.Half)MissingSingleMethods.Round((float)x, mode);

    [Pure]
    public static HalfAnalyzer.Half Round(HalfAnalyzer.Half x, [ValueRange(0, 6)] int digits, MidpointRounding mode)
        => (HalfAnalyzer.Half)MissingSingleMethods.Round((float)x, digits, mode);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out HalfAnalyzer.Half result)
        => HalfAnalyzer.Half.TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out HalfAnalyzer.Half result)
        => HalfAnalyzer.Half.TryParse(s.ToString(), NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out HalfAnalyzer.Half result) => HalfAnalyzer.Half.TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out HalfAnalyzer.Half result)
        => HalfAnalyzer.Half.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out HalfAnalyzer.Half result)
        => HalfAnalyzer.Half.TryParse(
            Encoding.UTF8.GetString(utf8Text.ToArray()),
            NumberStyles.Float | NumberStyles.AllowThousands,
            provider,
            out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out HalfAnalyzer.Half result)
        => HalfAnalyzer.Half.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}