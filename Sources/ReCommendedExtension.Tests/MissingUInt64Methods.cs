namespace ReCommendedExtension.Tests;

internal static class MissingUInt64Methods
{
    [Pure]
    public static ulong Clamp(ulong value, ulong min, ulong max)
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
    public static (ulong Quotient, ulong Remainder) DivRem(ulong left, [ValueRange(1, ulong.MaxValue)] ulong right)
    {
        var quotient = left / right;
        return (quotient, left - quotient * right);
    }

    [Pure]
    public static ulong Max(ulong x, ulong y) => x >= y ? x : y;

    [Pure]
    public static ulong Min(ulong x, ulong y) => x <= y ? x : y;
}