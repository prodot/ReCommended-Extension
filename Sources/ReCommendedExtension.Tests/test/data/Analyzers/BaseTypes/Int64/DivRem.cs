using System;

namespace Test
{
    public class Int64s
    {
        public void ExpressionResult(long left)
        {
            var result11 = long.DivRem(0, 10);
            var result12 = long.DivRem(left, 1);

            var result21 = Math.DivRem(0L, 10L);
            var result22 = Math.DivRem(left, 1L);
        }

        public void NoDetection(long left, long right)
        {
            var result11 = long.DivRem(0, right);
            var result12 = long.DivRem(left, 2);

            var result21 = Math.DivRem(0L, right);
            var result22 = Math.DivRem(left, 2L);

            long.DivRem(0, 10);
            long.DivRem(left, 1);

            Math.DivRem(0L, 10L);
            Math.DivRem(left, 1L);
        }
    }
}