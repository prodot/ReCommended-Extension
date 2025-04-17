using System;

namespace Test
{
    public class IntPtrs
    {
        public void ExpressionResult(nint left)
        {
            var result11 = nint.DivRem(0, 10);
            var result12 = nint.DivRem(left, 1);

            var result21 = Math.DivRem((nint)0, (nint)10);
            var result22 = Math.DivRem(left, (nint)1);
        }

        public void NoDetection(nint left, nint right)
        {
            var result11 = nint.DivRem(0, right);
            var result12 = nint.DivRem(left, 2);

            var result21 = Math.DivRem((nint)0, right);
            var result22 = Math.DivRem(left, (nint)2);

            nint.DivRem(0, 10);
            nint.DivRem(left, 1);

            Math.DivRem((nint)0, (nint)10);
            Math.DivRem(left, (nint)1);
        }
    }
}