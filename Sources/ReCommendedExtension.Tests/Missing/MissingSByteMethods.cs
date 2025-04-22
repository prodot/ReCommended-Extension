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
    public static sbyte Max(sbyte x, sbyte y) => x >= y ? x : y;

    [Pure]
    public static sbyte Min(sbyte x, sbyte y) => x <= y ? x : y;

    [Pure]
    public static sbyte RotateLeft(sbyte value, int rotateAmount)
        => unchecked((sbyte)(value << (rotateAmount & 7) | (byte)value >> (8 - rotateAmount & 7)));

    [Pure]
    public static sbyte RotateRight(sbyte value, int rotateAmount)
        => unchecked((sbyte)((byte)value >> (rotateAmount & 7) | value << (8 - rotateAmount & 7)));
}