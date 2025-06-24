using System;

namespace Test
{
    public class Randoms
    {
        public void GetItems(Random random, int[] array, ReadOnlySpan<int> span)
        {
            var result11 = random.GetItems(array, 0);
            var result12 = random.GetItems(span, 0);

            var result21 = random.GetItems<int>(array, 0);
            var result22 = random.GetItems<int>(span, 0);
        }

        public void GetItems<T>(Random random, T[] array, ReadOnlySpan<T> span)
        {
            var result11 = random.GetItems(array, 0);
            var result12 = random.GetItems(span, 0);

            var result21 = random.GetItems<T>(array, 0);
            var result22 = random.GetItems<T>(span, 0);
        }

        public void Next(Random random)
        {
            var result11 = random.Next(0);
            var result12 = random.Next(1);

            var result21 = random.Next(10, 10);
            var result22 = random.Next(10, 11);
        }

        public void NextInt64(Random random)
        {
            var result11 = random.NextInt64(0);
            var result12 = random.NextInt64(1);

            var result21 = random.NextInt64(10, 10);
            var result22 = random.NextInt64(10, 11);
        }
    }
}