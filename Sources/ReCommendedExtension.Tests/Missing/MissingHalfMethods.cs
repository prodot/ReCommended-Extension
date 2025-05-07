using System.Globalization;
using System.Text;
using ReCommendedExtension.Analyzers.BaseTypes.Analyzers.NumberInfos;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingHalfMethods
{
    [Pure]
    public static Half Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => Half.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Float | NumberStyles.AllowThousands, provider);

    [Pure]
    public static Half Parse(
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands,
        IFormatProvider? provider = null)
        => Half.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static Half Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => Half.Parse(s.ToString(), NumberStyles.Float | NumberStyles.AllowThousands, provider);

    [Pure]
    public static Half Parse(
        ReadOnlySpan<char> s,
        NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands,
        IFormatProvider? provider = null)
        => Half.Parse(s.ToString(), style, provider);

    [Pure]
    public static Half Round(Half x) => (Half)MissingSingleMethods.Round((float)x);

    [Pure]
    public static Half Round(Half x, [ValueRange(0, 6)] int digits) => (Half)MissingSingleMethods.Round((float)x, digits);

    [Pure]
    public static Half Round(Half x, MidpointRounding mode) => (Half)MissingSingleMethods.Round((float)x, mode);

    [Pure]
    public static Half Round(Half x, [ValueRange(0, 6)] int digits, MidpointRounding mode)
        => (Half)MissingSingleMethods.Round((float)x, digits, mode);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out Half result)
        => Half.TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Half result)
        => Half.TryParse(s.ToString(), NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out Half result) => Half.TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out Half result)
        => Half.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out Half result)
        => Half.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out Half result)
        => Half.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}