using System;

namespace Test
{
    public class Int64s
    {
        public void ExpressionResult(long number)
        {
            var result11 = long.Clamp(number, 1, 1);
            var result12 = long.Clamp(number, long.MinValue, long.MaxValue);

            var result21 = Math.Clamp(number, 1L, 1L);
            var result22 = Math.Clamp(number, long.MinValue, long.MaxValue);
        }

        public void NoDetection(long number, long min, long max)
        {
            var result11 = long.Clamp(number, 1, max);
            var result12 = long.Clamp(number, min, 10);

            var result21 = Math.Clamp(number, (long)1, max);
            var result22 = Math.Clamp(number, min, long.MaxValue);

            long.Clamp(number, 1, 1);
            long.Clamp(number, long.MinValue, long.MaxValue);

            Math.Clamp(number, 1L, 1L);
            Math.Clamp(number, long.MinValue, long.MaxValue);
        }
    }
}