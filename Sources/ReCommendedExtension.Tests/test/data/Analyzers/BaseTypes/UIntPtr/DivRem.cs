using System;

namespace Test
{
    public class UIntPtrs
    {
        public void ExpressionResult(nuint left)
        {
            var result11 = nuint.DivRem(0, 10);
            var result12 = nuint.DivRem(left, 1);

            var result21 = Math.DivRem((nuint)0, (nuint)10);
            var result22 = Math.DivRem(left, (nuint)1);
        }

        public void NoDetection(nuint left, nuint right)
        {
            var result11 = nuint.DivRem(0, right);
            var result12 = nuint.DivRem(left, 2);

            var result21 = Math.DivRem((nuint)0, right);
            var result22 = Math.DivRem(left, (nuint)2);

            nuint.DivRem(0, 10);
            nuint.DivRem(left, 1);

            Math.DivRem((nuint)0, (nuint)10);
            Math.DivRem(left, (nuint)1);
        }
    }
}