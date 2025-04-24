using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingDecimalMethods
{
    [Pure]
    public static decimal Clamp(decimal value, decimal min, decimal max)
    {
        if (min > max)
        {
            throw new ArgumentException($"'{min}' cannot be greater than {max}.");
        }

        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }

    [Pure]
    public static bool IsNegative(decimal value) => value < 0;

    [Pure]
    public static bool IsPositive(decimal value) => value >= 0;

    [Pure]
    public static decimal Max(decimal x, decimal y) => x >= y ? x : y;

    [Pure]
    public static decimal Min(decimal x, decimal y) => x <= y ? x : y;

    [Pure]
    public static decimal Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => decimal.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Number, provider);

    [Pure]
    public static decimal Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null)
        => decimal.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static decimal Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => decimal.Parse(s.ToString(), NumberStyles.Number, provider);

    [Pure]
    public static decimal Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Number, IFormatProvider? provider = null)
        => decimal.Parse(s.ToString(), style, provider);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out decimal result)
        => decimal.TryParse(s, NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out decimal result)
        => decimal.TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out decimal result)
        => decimal.TryParse(s.ToString(), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out decimal result) => decimal.TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out decimal result)
        => decimal.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out decimal result)
        => decimal.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out decimal result)
        => decimal.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}