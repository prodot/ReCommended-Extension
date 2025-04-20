namespace ReCommendedExtension.Tests.Missing;

internal static class MissingIntPtrMethods
{
    [Pure]
    public static nint Clamp(nint value, nint min, nint max)
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
    public static (nint Quotient, nint Remainder) DivRem(nint left, nint right)
    {
        var quotient = left / right;
        return (quotient, left - quotient * right);
    }

    [Pure]
    public static nint Max(nint x, nint y) => x >= y ? x : y;

    [Pure]
    public static nint Min(nint x, nint y) => x <= y ? x : y;
}