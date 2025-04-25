using System.Globalization;
using System.Text;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingSByteMethods
{
    [Pure]
    public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
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
    public static (sbyte Quotient, sbyte Remainder) DivRem(sbyte left, [ValueRange(sbyte.MinValue, -1)][ValueRange(1, sbyte.MaxValue)] sbyte right)
    {
        var quotient = unchecked((sbyte)(left / right));
        return (quotient, unchecked((sbyte)(left - quotient * right)));
    }

    [Pure]
    public static bool IsNegative(sbyte value) => value < 0;

    [Pure]
    public static bool IsPositive(sbyte value) => value >= 0;

    [Pure]
    public static sbyte Max(sbyte x, sbyte y) => x >= y ? x : y;

    [Pure]
    public static sbyte Min(sbyte x, sbyte y) => x <= y ? x : y;

    [Pure]
    public static sbyte MaxMagnitude(sbyte x, sbyte y)
    {
        if (x == sbyte.MinValue)
        {
            return x;
        }

        if (y == sbyte.MinValue)
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
    public static sbyte MinMagnitude(sbyte x, sbyte y)
    {
        if (x == sbyte.MinValue)
        {
            return y;
        }

        if (y == sbyte.MinValue)
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
    public static sbyte Parse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider)
        => sbyte.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Integer, provider);

    [Pure]
    public static sbyte Parse(ReadOnlySpan<byte> utf8Text, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => sbyte.Parse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider);

    [Pure]
    public static sbyte Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => sbyte.Parse(s.ToString(), NumberStyles.Integer, provider);

    [Pure]
    public static sbyte Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Integer, IFormatProvider? provider = null)
        => sbyte.Parse(s.ToString(), style, provider);

    [Pure]
    public static sbyte RotateLeft(sbyte value, int rotateAmount)
        => unchecked((sbyte)(value << (rotateAmount & 7) | (byte)value >> (8 - rotateAmount & 7)));

    [Pure]
    public static sbyte RotateRight(sbyte value, int rotateAmount)
        => unchecked((sbyte)((byte)value >> (rotateAmount & 7) | value << (8 - rotateAmount & 7)));

    [Pure]
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out sbyte result)
        => sbyte.TryParse(s, NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out sbyte result)
        => sbyte.TryParse(s.ToString(), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out sbyte result)
        => sbyte.TryParse(s.ToString(), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<char> s, out sbyte result) => sbyte.TryParse(s.ToString(), out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider? provider, out sbyte result)
        => sbyte.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), style, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider? provider, out sbyte result)
        => sbyte.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), NumberStyles.Number, provider, out result);

    [Pure]
    public static bool TryParse(ReadOnlySpan<byte> utf8Text, out sbyte result)
        => sbyte.TryParse(Encoding.UTF8.GetString(utf8Text.ToArray()), out result);
}