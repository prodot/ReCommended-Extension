using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingDoubleMethods
{
    [Pure]
    public static double Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => double.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Float | NumberStyles.AllowThousands, provider);

    [Pure]
    public static double Parse(
        ReadOnlySpan<byte> utf8Text,
        NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands,
        IFormatProvider? provider = null)
        => double.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static double Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
        => double.Parse(s.ToString(), NumberStyles.Float | NumberStyles.AllowThousands, provider);

    [Pure]
    public static double Parse(
        ReadOnlySpan<char> s,
        NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands,
        IFormatProvider? provider = null)
        => double.Parse(s.ToString(), style, provider);

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