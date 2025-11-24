namespace ReCommendedExtension.Tests.Missing;

internal static class MissingRandomMethods
{
    extension(Random random)
    {
        public string GetHexString([NonNegativeValue] int stringLength, bool lowercase = false)
            => stringLength switch
            {
                < 0 => throw new ArgumentOutOfRangeException(nameof(stringLength)),
                0 => "",
                _ => new string(random.GetItems((lowercase ? "0123456789abcdef" : "0123456789ABCDEF").AsSpan(), stringLength)),
            };

        public T[] GetItems<T>(T[] choices, [NonNegativeValue] int length) => random.GetItems(new ReadOnlySpan<T>(choices), length);

        public T[] GetItems<T>(ReadOnlySpan<T> choices, [NonNegativeValue] int length)
        {
            if (choices.IsEmpty)
            {
                throw new ArgumentException("Span cannot be empty.", nameof(choices));
            }

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