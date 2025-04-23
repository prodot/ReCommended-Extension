using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingInt64Methods
{
    [Pure]
    public static long Clamp(long value, long min, long max)
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
    public static (long Quotient, long Remainder) DivRem(long left, [ValueRange(long.MinValue, -1)][ValueRange(1, long.MaxValue)] long right)
    {
        var quotient = left / right;
        return (quotient, left - quotient * right);
    }

    [Pure]
    public static long Max(long x, long y) => x >= y ? x : y;

    [Pure]
    public static long Min(long x, long y) => x <= y ? x : y;

    [Pure]
    public static long Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => long.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider);

    [Pure]
    public static long Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => long.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static long Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => long.Parse(s.ToString(), NumberStyles.Integer, provider);

    [Pure]
    public static long Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => long.Parse(s.ToString(), style, provider);

    [Pure]
    public static long RotateLeft(long value, int rotateAmount) => unchecked((long)MissingUInt64Methods.RotateLeft((ulong)value, rotateAmount));

    [Pure]
    public static long RotateRight(long value, int rotateAmount) => unchecked((long)MissingUInt64Methods.RotateRight((ulong)value, rotateAmount));

}