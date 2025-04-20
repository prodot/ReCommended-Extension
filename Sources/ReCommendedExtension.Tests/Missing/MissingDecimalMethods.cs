namespace ReCommendedExtension.Tests.Missing;

internal static class MissingDecimalMethods
{
    [Pure]
    public static decimal Clamp(decimal value, decimal min, decimal max)
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
    public static decimal Max(decimal x, decimal y) => x >= y ? x : y;

    [Pure]
    public static decimal Min(decimal x, decimal y) => x <= y ? x : y;
}