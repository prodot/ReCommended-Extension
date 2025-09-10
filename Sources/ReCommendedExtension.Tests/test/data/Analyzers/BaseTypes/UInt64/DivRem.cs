using System;

namespace Test
{
    public class UInt64s
    {
        public void ExpressionResult(ulong left)
        {
            var result11 = ulong.DivRem(0, 10);

            var result21 = Math.DivRem(0ul, 10ul);
        }

        public void NoDetection(ulong left, ulong right)
        {
            var result11 = ulong.DivRem(0, right);

            var result21 = Math.DivRem(0ul, right);

            ulong.DivRem(0, 10);

            Math.DivRem(0ul, 10ul);
        }
    }
}