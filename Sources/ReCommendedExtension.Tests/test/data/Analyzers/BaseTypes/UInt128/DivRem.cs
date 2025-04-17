using System;

namespace Test
{
    public class UInt128s
    {
        public void ExpressionResult(UInt128 left)
        {
            var result1 = UInt128.DivRem(0, 10);
            var result2 = UInt128.DivRem(left, 1);
        }

        public void NoDetection(UInt128 left, UInt128 right)
        {
            var result1 = UInt128.DivRem(0, right);
            var result2 = UInt128.DivRem(left, 2);

            UInt128.DivRem(0, 10);
            UInt128.DivRem(left, 1);
        }
    }
}