namespace ReCommendedExtension.Tests.Missing;

internal static class MissingUIntPtrMethods
{
    [Pure]
    public static nuint Clamp(nuint value, nuint min, nuint max)
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
    public static (nuint Quotient, nuint Remainder) DivRem(nuint left, nuint right)
    {
        var quotient = left / right;
        return (quotient, left - quotient * right);
    }

    [Pure]
    public static nuint Max(nuint x, nuint y) => x >= y ? x : y;

    [Pure]
    public static nuint Min(nuint x, nuint y) => x <= y ? x : y;
}