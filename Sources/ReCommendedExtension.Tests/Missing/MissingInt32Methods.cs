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
    public static bool IsNegative(int value) => value < 0;

    [Pure]
    public static bool IsPositive(int value) => value >= 0;

    [Pure]
    public static int Max(int x, int y) => x >= y ? x : y;

    [Pure]
    public static int Min(int x, int y) => x <= y ? x : y;

    [Pure]
    public static int MaxMagnitude(int x, int y)
    {
        if (x == int.MinValue)
        {
            return x;
        }

        if (y == int.MinValue)
        {
            return y;
        }

        var ax = Math.Abs(x);
        var ay = Math.Abs(y);

        if (ax > ay)
        {
            return x;
        }

        if (ax < ay)
        {
            return y;
        }

        return x < 0 ? y : x;
    }

    [Pure]
    public static int MinMagnitude(int x, int y)
    {
        if (x == int.MinValue)
        {
            return y;
        }

        if (y == int.MinValue)
        {
            return x;
        }

        var ax = Math.Abs(x);
        var ay = Math.Abs(y);

        if (ax < ay)
        {
            return x;
        }

        if (ax > ay)
        {
            return y;
        }

        return x < 0 ? x : y;
    }

    [Pure]
    public static int Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => int.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), provider);

    [Pure]
    public static int Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => int.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static int Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => int.Parse(s.ToString(), provider);

    [Pure]
    public static int Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => int.Parse(s.ToString(), style, provider);

    [Pure]
    public static int RotateLeft(int value, int rotateAmount) => unchecked((int)MissingUInt32Methods.RotateLeft((uint)value, rotateAmount));

    [Pure]
    public static int RotateRight(int value, int rotateAmount) => unchecked((int)MissingUInt32Methods.RotateRight((uint)value, rotateAmount));

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out int result)
        => int.TryParse(s, NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out int result)
        => int.TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out int result)
        => int.TryParse(s.ToString(), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out int result) => int.TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out int result)
        => int.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out int result)
        => int.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out int result) => int.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}