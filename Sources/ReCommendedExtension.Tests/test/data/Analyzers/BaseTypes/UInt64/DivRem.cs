using System;

namespace Test
{
    public class UInt64s
    {
        public void ExpressionResult(ulong left)
        {
            var result11 = ulong.DivRem(0, 10);
            var result12 = ulong.DivRem(left, 1);

            var result21 = Math.DivRem(0ul, 10ul);
            var result22 = Math.DivRem(left, 1ul);
        }

        public void NoDetection(ulong left, ulong right)
        {
            var result11 = ulong.DivRem(0, right);
            var result12 = ulong.DivRem(left, 2);

            var result21 = Math.DivRem(0ul, right);
            var result22 = Math.DivRem(left, 2ul);

            ulong.DivRem(0, 10);
            ulong.DivRem(left, 1);

            Math.DivRem(0ul, 10ul);
            Math.DivRem(left, 1ul);
        }
    }
}