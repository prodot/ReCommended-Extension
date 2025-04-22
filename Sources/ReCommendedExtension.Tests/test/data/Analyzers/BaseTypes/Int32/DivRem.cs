using System;

namespace Test
{
    public class Int32s
    {
        public void ExpressionResult(int left)
        {
            var result11 = int.DivRem(0, 10);

            var result21 = Math.DivRem(0, 10);
        }

        public void NoDetection(int left, int right)
        {
            var result11 = int.DivRem(0, right);

            var result21 = Math.DivRem(0, right);

            int.DivRem(0, 10);

            Math.DivRem(0, 10);
        }
    }
}