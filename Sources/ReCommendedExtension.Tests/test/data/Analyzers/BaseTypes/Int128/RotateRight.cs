using System;

namespace Test
{
    public class Int128s
    {
        public void ExpressionResult(Int128 n)
        {
            var result = Int128.RotateRight(n, 0);
        }

        public void NoDetection(Int128 n, int rotateAmount)
        {
            var result1 = Int128.RotateRight(n, 1);
            var result2 = Int128.RotateRight(n, rotateAmount);

            Int128.RotateRight(n, 0);
        }
    }
}