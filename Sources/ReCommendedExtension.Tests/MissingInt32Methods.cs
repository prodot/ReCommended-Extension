namespace ReCommendedExtension.Tests;

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
}