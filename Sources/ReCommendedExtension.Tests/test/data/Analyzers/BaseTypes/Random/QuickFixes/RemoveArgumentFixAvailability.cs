using System;

namespace Test
{
    public class Randoms
    {
        public void Next(Random random, int maxValue)
        {
            var result11 = random.Next(int.MaxValue);
            var result12 = random.Next(2_147_483_647);
            var result13 = random.Next(0x7fff_ffff);

            var result21 = random.Next(0, maxValue);
        }

        public void Next_Nullable(Random? random, int maxValue)
        {
            var result11 = random?.Next(int.MaxValue);
            var result12 = random?.Next(2_147_483_647);
            var result13 = random?.Next(0x7fff_ffff);

            var result21 = random?.Next(0, maxValue);
        }

        public void NextInt64(Random random, long maxValue)
        {
            var result11 = random.NextInt64(long.MaxValue);
            var result12 = random.NextInt64(9_223_372_036_854_775_807);
            var result13 = random.NextInt64(0x7fff_ffff_ffff_ffff);

            var result21 = random.NextInt64(0, maxValue);
        }

        public void NextInt64_Nullable(Random? random, int maxValue)
        {
            var result11 = random?.NextInt64(long.MaxValue);
            var result12 = random?.NextInt64(9_223_372_036_854_775_807);
            var result13 = random?.NextInt64(0x7fff_ffff_ffff_ffff);

            var result21 = random?.NextInt64(0, maxValue);
        }
    }
}