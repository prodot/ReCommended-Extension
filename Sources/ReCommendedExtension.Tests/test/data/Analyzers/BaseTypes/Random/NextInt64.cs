using System;

namespace Test
{
    public class Randoms
    {
        public void RedundantArgument(Random random, long maxValue)
        {
            var result11 = random.NextInt64(long.MaxValue);
            var result12 = random.NextInt64(9_223_372_036_854_775_807);
            var result13 = random.NextInt64(0x7fff_ffff_ffff_ffff);

            var result21 = random.NextInt64(0, maxValue);
        }

        public void RedundantArgument_Nullable(Random? random, int maxValue)
        {
            var result11 = random?.NextInt64(long.MaxValue);
            var result12 = random?.NextInt64(9_223_372_036_854_775_807);
            var result13 = random?.NextInt64(0x7fff_ffff_ffff_ffff);

            var result21 = random?.NextInt64(0, maxValue);
        }

        public void ExpressionResult(Random random)
        {
            var result11 = random.NextInt64(0);
            var result12 = random.NextInt64(1);

            var result21 = random.NextInt64(10, 10);
            var result22 = random.NextInt64(10, 11);
        }

        public void NoDetection(Random random, long minValue, long maxValue)
        {
            var result1 = random.NextInt64(minValue, int.MaxValue);
            var result2 = random.NextInt64(minValue, maxValue);

            random.NextInt64(0);
            random.NextInt64(1);

            random.NextInt64(10, 10);
            random.NextInt64(10, 11);
        }

        public void NoDetection_Nullable(Random? random)
        {
            var result11 = random?.NextInt64(0);
            var result12 = random?.NextInt64(1);

            var result21 = random?.NextInt64(10, 10);
            var result22 = random?.NextInt64(10, 11);
        }
    }
}