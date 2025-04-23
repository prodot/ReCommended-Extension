using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingInt32Methods
{
    [Pure]
    public static int Clamp(int value, int min, int max)
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
    public static (int Quotient, int Remainder) DivRem(int left, [ValueRange(int.MinValue, -1)][ValueRange(1, int.MaxValue)] int right)
    {
        var quotient = left / right;
        return (quotient, left - quotient * right);
    }

    [Pure]
    public static int Max(int x, int y) => x >= y ? x : y;

    [Pure]
    public static int Min(int x, int y) => x <= y ? x : y;

    [Pure]
    public static int Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => int.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider);

    [Pure]
    public static int Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => int.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static int Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => int.Parse(s.ToString(), NumberStyles.Integer, provider);

    [Pure]
    public static int Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => int.Parse(s.ToString(), style, provider);

    [Pure]
    public static int RotateLeft(int value, int rotateAmount) => unchecked((int)MissingUInt32Methods.RotateLeft((uint)value, rotateAmount));

    [Pure]
    public static int RotateRight(int value, int rotateAmount) => unchecked((int)MissingUInt32Methods.RotateRight((uint)value, rotateAmount));
}