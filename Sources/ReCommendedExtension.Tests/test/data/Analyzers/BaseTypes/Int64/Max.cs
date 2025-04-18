using System;

namespace Test
{
    public class Int64s
    {
        public void ExpressionResult(long number)
        {
            var result1 = long.Max(10, 10);

            var result2 = Math.Max(10L, 10L);
        }

        public void NoDetection(long x, long y)
        {
            var result11 = long.Max(1, 2);
            var result12 = long.Max(x, 2);
            var result13 = long.Max(1, y);

            long.Max(10, 10);

            Math.Max(10L, 10L);
        }
    }
}