using System;

namespace Test
{
    public class Int128s
    {
        public void ExpressionResult(Int128 left)
        {
            var result1 = Int128.DivRem(0, 10);
            var result2 = Int128.DivRem(left, 1);
        }

        public void NoDetection(Int128 left, Int128 right)
        {
            var result1 = Int128.DivRem(0, right);
            var result2 = Int128.DivRem(left, 2);

            Int128.DivRem(0, 10);
            Int128.DivRem(left, 1);
        }
    }
}