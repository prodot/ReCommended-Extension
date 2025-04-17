namespace ReCommendedExtension.Tests;

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
}