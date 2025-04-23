using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingUInt32Methods
{
    [Pure]
    public static uint Clamp(uint value, uint min, uint max)
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
    public static (uint Quotient, uint Remainder) DivRem(uint left, [ValueRange(1, uint.MaxValue)] uint right)
    {
        var quotient = left / right;
        return (quotient, left - quotient * right);
    }

    [Pure]
    public static uint Max(uint x, uint y) => x >= y ? x : y;

    [Pure]
    public static uint Min(uint x, uint y) => x <= y ? x : y;

    [Pure]
    public static uint Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => uint.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider);

    [Pure]
    public static uint Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => uint.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static uint Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => uint.Parse(s.ToString(), NumberStyles.Integer, provider);

    [Pure]
    public static uint Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => uint.Parse(s.ToString(), style, provider);

    [Pure]
    public static uint RotateLeft(uint value, int offset) => value << offset | value >> (32 - offset);

    [Pure]
    public static uint RotateRight(uint value, int offset) => value >> offset | value << (32 - offset);
}