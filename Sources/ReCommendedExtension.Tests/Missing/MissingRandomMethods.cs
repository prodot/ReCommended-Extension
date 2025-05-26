namespace ReCommendedExtension.Tests.Missing;

internal static class MissingRandomMethods
{
    public static T[] GetItems<T>(this Random random, T[] choices, [NonNegativeValue] int length)
        => random.GetItems(new ReadOnlySpan<T>(choices), length);

    public static T[] GetItems<T>(this Random random, ReadOnlySpan<T> choices, [NonNegativeValue] int length)
    {
        var items = new T[length];

        for (var i = 0; i < items.Length; i++)
        {
            items[i] = choices[random.Next(choices.Length)];
        }

        return items;
    }

    public static long NextInt64(this Random random, long minValue, long maxValue)
    {
        if (minValue > maxValue)
        {
            throw new ArgumentOutOfRangeException();
        }

        if (minValue == maxValue)
        {
            return minValue;
        }

        if (minValue >= int.MinValue && maxValue <= int.MaxValue)
        {
            return random.Next(unchecked((int)minValue), unchecked((int)maxValue));
        }

        var bytes = new byte[sizeof(ulong)];
        random.NextBytes(bytes);

        return unchecked((long)(BitConverter.ToUInt64(bytes, 0) % (ulong)(maxValue - minValue)) + minValue);
    }

    public static long NextInt64(this Random random, [NonNegativeValue] long maxValue) => random.NextInt64(0, maxValue);

    public static long NextInt64(this Random random) => random.NextInt64(0, long.MaxValue);
}