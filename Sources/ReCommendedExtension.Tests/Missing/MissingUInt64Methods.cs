using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingUInt64Methods
{
    [Pure]
    public static ulong Clamp(ulong value, ulong min, ulong max)
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
    public static (ulong Quotient, ulong Remainder) DivRem(ulong left, [ValueRange(1, ulong.MaxValue)] ulong right)
    {
        var quotient = left / right;
        return (quotient, left - quotient * right);
    }

    [Pure]
    public static ulong Max(ulong x, ulong y) => x >= y ? x : y;

    [Pure]
    public static ulong Min(ulong x, ulong y) => x <= y ? x : y;

    [Pure]
    public static ulong Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => ulong.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), provider);

    [Pure]
    public static ulong Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => ulong.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static ulong Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => ulong.Parse(s.ToString(), provider);

    [Pure]
    public static ulong Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => ulong.Parse(s.ToString(), style, provider);

    [Pure]
    public static ulong RotateLeft(ulong value, int rotateAmount) => value << rotateAmount | value >> (64 - rotateAmount);

    [Pure]
    public static ulong RotateRight(ulong value, int rotateAmount) => value >> rotateAmount | value << (64 - rotateAmount);

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ulong result)
        => ulong.TryParse(s, NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out ulong result)
        => ulong.TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ulong result)
        => ulong.TryParse(s.ToString(), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out ulong result) => ulong.TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out ulong result)
        => ulong.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out ulong result)
        => ulong.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out ulong result)
        => ulong.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}