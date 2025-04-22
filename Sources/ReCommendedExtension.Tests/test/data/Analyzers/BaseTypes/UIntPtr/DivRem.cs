using System;

namespace Test
{
    public class UIntPtrs
    {
        public void ExpressionResult(nuint left)
        {
            var result11 = nuint.DivRem(0, 10);

            var result21 = Math.DivRem((nuint)0, (nuint)10);
        }

        public void NoDetection(nuint left, nuint right)
        {
            var result11 = nuint.DivRem(0, right);

            var result21 = Math.DivRem((nuint)0, right);

            nuint.DivRem(0, 10);

            Math.DivRem((nuint)0, (nuint)10);
        }
    }
}