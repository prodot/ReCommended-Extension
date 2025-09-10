using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingDoubleMethods
{
    [Pure]
    public static double Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => double.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), provider);

    [Pure]
    public static double Parse(
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands,
        IFormatProvider? provider = null)
        => double.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static double Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => double.Parse(s.ToString(), provider);

    [Pure]
    public static double Parse(
        ReadOnlySpan<char> s,
        NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands,
        IFormatProvider? provider = null)
        => double.Parse(s.ToString(), style, provider);

    [Pure]
    public static double Round(double x) => Math.Round(x);

    [Pure]
    public static double Round(double x, [ValueRange(0, 15)] int digits) => Math.Round(x, digits);

    [Pure]
    public static double Round(double x, MidpointRounding mode) => Math.Round(x, mode);

    [Pure]
    public static double Round(double x, [ValueRange(0, 15)] int digits, MidpointRounding mode) => Math.Round(x, digits, mode);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out double result)
        => double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out double result)
        => double.TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out double result)
        => double.TryParse(s.ToString(), NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out double result) => double.TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out double result)
        => double.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out double result)
        => double.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out double result)
        => double.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}