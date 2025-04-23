using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingUInt16Methods
{
    [Pure]
    public static ushort Clamp(ushort value, ushort min, ushort max)
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
    public static (ushort Quotient, ushort Remainder) DivRem(ushort left, [ValueRange(1, ushort.MaxValue)] ushort right)
    {
        var quotient = unchecked((ushort)(left / right));
        return (quotient, unchecked((ushort)(left - quotient * right)));
    }

    [Pure]
    public static ushort Max(ushort x, ushort y) => x >= y ? x : y;

    [Pure]
    public static ushort Min(ushort x, ushort y) => x <= y ? x : y;

    [Pure]
    public static ushort Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => ushort.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider);

    [Pure]
    public static ushort Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => ushort.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static ushort Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => ushort.Parse(s.ToString(), NumberStyles.Integer, provider);

    [Pure]
    public static ushort Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => ushort.Parse(s.ToString(), style, provider);

    [Pure]
    public static ushort RotateLeft(ushort value, int rotateAmount)
        => unchecked((ushort)(value << (rotateAmount & 15) | value >> (16 - rotateAmount & 15)));

    [Pure]
    public static ushort RotateRight(ushort value, int rotateAmount)
        => unchecked((ushort)(value >> (rotateAmount & 15) | value << (16 - rotateAmount & 15)));
}