using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingByteMethods
{
    [Pure]
    public static byte Clamp(byte value, byte min, byte max)
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
    public static (byte Quotient, byte Remainder) DivRem(byte left, [ValueRange(1, byte.MaxValue)] byte right)
    {
        var quotient = unchecked((byte)(left / right));
        return (quotient, unchecked((byte)(left - quotient * right)));
    }

    [Pure]
    public static byte Max(byte x, byte y) => x >= y ? x : y;

    [Pure]
    public static byte Min(byte x, byte y) => x <= y ? x : y;

    [Pure]
    public static byte Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => byte.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), provider);

    [Pure]
    public static byte Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => byte.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static byte Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => byte.Parse(s.ToString(), provider);

    [Pure]
    public static byte Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => byte.Parse(s.ToString(), style, provider);

    [Pure]
    public static byte RotateLeft(byte value, int rotateAmount) => unchecked((byte)(value << (rotateAmount & 7) | value >> (8 - rotateAmount & 7)));

    [Pure]
    public static byte RotateRight(byte value, int rotateAmount) => unchecked((byte)(value >> (rotateAmount & 7) | value << (8 - rotateAmount & 7)));

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out byte result)
        => byte.TryParse(s, NumberStyles.Integer, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out byte result)
        => byte.TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out byte result)
        => byte.TryParse(s.ToString(), NumberStyles.Integer, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out byte result) => byte.TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out byte result)
        => byte.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out byte result)
        => byte.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out byte result)
        => byte.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}