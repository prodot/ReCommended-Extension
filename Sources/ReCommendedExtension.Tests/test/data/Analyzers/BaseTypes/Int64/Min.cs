using System;

namespace Test
{
    public class Int64s
    {
        public void ExpressionResult(long number)
        {
            var result1 = long.Min(10, 10);

            var result2 = Math.Min(10L, 10L);
        }

        public void NoDetection(long x, long y)
        {
            var result11 = long.Min(1, 2);
            var result12 = long.Min(x, 2);
            var result13 = long.Min(1, y);

            long.Min(10, 10);

            Math.Min(10L, 10L);
        }
    }
}