using System;

namespace Test
{
    public class Int64s
    {
        public void ExpressionResult(long left)
        {
            var result11 = long.DivRem(0, 10);

            var result21 = Math.DivRem(0L, 10L);
        }

        public void NoDetection(long left, long right)
        {
            var result11 = long.DivRem(0, right);

            var result21 = Math.DivRem(0L, right);

            long.DivRem(0, 10);

            Math.DivRem(0L, 10L);
        }
    }
}