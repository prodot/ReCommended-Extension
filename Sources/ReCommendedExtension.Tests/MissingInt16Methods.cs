namespace ReCommendedExtension.Tests;

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
}