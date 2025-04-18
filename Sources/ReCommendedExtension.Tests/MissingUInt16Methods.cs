namespace ReCommendedExtension.Tests;

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
}