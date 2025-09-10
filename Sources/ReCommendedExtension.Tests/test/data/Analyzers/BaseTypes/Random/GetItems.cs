using System;

namespace Test
{
    public class Randoms
    {
        public void ExpressionResult(Random random, int[] array, ReadOnlySpan<int> span)
        {
            var result11 = random.GetItems(array, 0);
            var result12 = random.GetItems(span, 0);

            var result21 = random.GetItems<int>(array, 0);
            var result22 = random.GetItems<int>(span, 0);
        }

        public void ExpressionResult<T>(Random random, T[] array, ReadOnlySpan<T> span)
        {
            var result11 = random.GetItems(array, 0);
            var result12 = random.GetItems(span, 0);

            var result21 = random.GetItems<T>(array, 0);
            var result22 = random.GetItems<T>(span, 0);
        }

        public void NoDetection(Random random, int[] array, ReadOnlySpan<int> span, int length)
        {
            var result1 = random.GetItems(array, length);
            var result2 = random.GetItems(span, length);

            random.GetItems(array, 0);
            random.GetItems(span, 0);
        }

        public void NoDetection_NullableArray(Random random, int[]? choices)
        {
            var result = random.GetItems(choices, 0);
        }

        public void NoDetection_NullableRandom(Random? random, int[] choices)
        {
            var result = random.GetItems(choices, 0);
        }

        public void NoDetection_NullableRandom(Random? random, ReadOnlySpan<int> choices)
        {
            var result = random.GetItems(choices, 0);
        }
    }
}