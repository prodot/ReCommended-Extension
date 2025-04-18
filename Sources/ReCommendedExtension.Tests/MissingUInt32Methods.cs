namespace ReCommendedExtension.Tests;

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
}