using System;

namespace Test
{
    public class IntPtrs
    {
        public void ExpressionResult(nint left)
        {
            var result11 = nint.DivRem(0, 10);

            var result21 = Math.DivRem((nint)0, (nint)10);
        }

        public void NoDetection(nint left, nint right)
        {
            var result11 = nint.DivRem(0, right);

            var result21 = Math.DivRem((nint)0, right);

            nint.DivRem(0, 10);

            Math.DivRem((nint)0, (nint)10);
        }
    }
}