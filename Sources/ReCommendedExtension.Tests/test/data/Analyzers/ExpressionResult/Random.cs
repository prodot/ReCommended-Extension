using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(Random random, int[] array, ReadOnlySpan<int> span)
        {
            var result11 = random.GetItems(array, 0);
            var result12 = random.GetItems(span, 0);

            var result21 = random.Next(0);
            var result22 = random.Next(1);
            var result23 = random.Next(10, 10);
            var result24 = random.Next(10, 11);

            var result31 = random.NextInt64(0);
            var result32 = random.NextInt64(1);
            var result33 = random.NextInt64(10, 10);
            var result34 = random.NextInt64(10, 11);
        }

        public void ExpressionResult<T>(Random random, T[] array, ReadOnlySpan<T> span)
        {
            var result11 = random.GetItems(array, 0);
            var result12 = random.GetItems(span, 0);
        }

        public void NoDetection(Random random, Random? randomNullable, int[] array, int[]? arrayNullable, ReadOnlySpan<int> span, int length, int minValue, int maxValue)
        {
            var result11 = random.GetItems(array, length);
            var result12 = random.GetItems(arrayNullable, 0);
            var result13 = randomNullable?.GetItems(array, 0);
            var result14 = random.GetItems(span, length);
            var result15 = randomNullable?.GetItems(span, 0);

            var result21 = random.Next(maxValue);
            var result22 = randomNullable?.Next(maxValue);
            var result23 = random.Next(minValue, maxValue);
            var result24 = randomNullable?.Next(minValue, maxValue);

            var result31 = random.NextInt64(maxValue);
            var result32 = randomNullable?.NextInt64(maxValue);
            var result33 = random.NextInt64(minValue, maxValue);
            var result34 = randomNullable?.NextInt64(minValue, maxValue);

            random.GetItems(array, 0);
            random.GetItems(span, 0);

            random.Next(0);
            random.Next(1);
            random.Next(10, 10);
            random.Next(10, 11);

            random.NextInt64(0);
            random.NextInt64(1);
            random.NextInt64(10, 10);
            random.NextInt64(10, 11);
        }
    }
}