namespace ReCommendedExtension.Tests.Missing;

internal static class MissingRandomMethods
{
    extension(Random random)
    {
        public T[] GetItems<T>(T[] choices, [NonNegativeValue] int length) => random.GetItems(new ReadOnlySpan<T>(choices), length);

        public T[] GetItems<T>(ReadOnlySpan<T> choices, [NonNegativeValue] int length)
        {
            var items = new T[length];

            for (var i = 0; i < items.Length; i++)
            {
                items[i] = choices[random.Next(choices.Length)];
            }

            return items;
        }

        public long NextInt64(long minValue, long maxValue)
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

        public long NextInt64([NonNegativeValue] long maxValue) => random.NextInt64(0, maxValue);

        public long NextInt64() => random.NextInt64(0, long.MaxValue);
    }
}