using System;

namespace Test
{
    public class UInt128s
    {
        public void ExpressionResult(UInt128 n)
        {
            var result = UInt128.RotateRight(n, 0);
        }

        public void NoDetection(UInt128 n, int rotateAmount)
        {
            var result1 = UInt128.RotateRight(n, 1);
            var result2 = UInt128.RotateRight(n, rotateAmount);

            UInt128.RotateRight(n, 0);
        }
    }
}