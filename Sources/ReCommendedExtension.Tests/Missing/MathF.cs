namespace ReCommendedExtension.Tests.Missing;

internal static class MathF
{
    [Pure]
    public static float Round(float x) => (float)Math.Round(x);

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    public static float Round(float x, [ValueRange(0, 6)] int digits)
    {
        if (digits is < 0 or > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(digits));
        }

        return (float)Math.Round(x, digits);
    }

    [Pure]
    public static float Round(float x, MidpointRounding mode) => (float)Math.Round(x, mode);

    [Pure]
    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    public static float Round(float x, [ValueRange(0, 6)] int digits, MidpointRounding mode)
    {
        if (digits is < 0 or > 6)
        {
            throw new ArgumentOutOfRangeException(nameof(digits));
        }

        return (float)Math.Round(x, digits, mode);
    }
}