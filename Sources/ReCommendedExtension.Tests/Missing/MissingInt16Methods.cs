using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingInt16Methods
{
    [Pure]
    public static short Clamp(short value, short min, short max)
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
    public static (short Quotient, short Remainder) DivRem(short left, [ValueRange(short.MinValue, -1)][ValueRange(1, short.MaxValue)] short right)
    {
        var quotient = unchecked((short)(left / right));
        return (quotient, unchecked((short)(left - quotient * right)));
    }

    [Pure]
    public static short Max(short x, short y) => x >= y ? x : y;

    [Pure]
    public static short Min(short x, short y) => x <= y ? x : y;

    [Pure]
    public static short Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => short.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider);

    [Pure]
    public static short Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => short.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static short Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => short.Parse(s.ToString(), NumberStyles.Integer, provider);

    [Pure]
    public static short Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => short.Parse(s.ToString(), style, provider);

    [Pure]
    public static short RotateLeft(short value, int rotateAmount)
        => unchecked((short)(value << (rotateAmount & 15) | (ushort)value >> (16 - rotateAmount & 15)));

    [Pure]
    public static short RotateRight(short value, int rotateAmount)
        => unchecked((short)((ushort)value >> (rotateAmount & 15) | value << (16 - rotateAmount & 15)));
}