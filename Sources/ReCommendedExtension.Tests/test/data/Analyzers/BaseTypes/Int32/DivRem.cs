using System;

namespace Test
{
    public class Int32s
    {
        public void ExpressionResult(int left)
        {
            var result11 = int.DivRem(0, 10);
            var result12 = int.DivRem(left, 1);

            var result21 = Math.DivRem(0, 10);
            var result22 = Math.DivRem(left, 1);
        }

        public void NoDetection(int left, int right)
        {
            var result11 = int.DivRem(0, right);
            var result12 = int.DivRem(left, 2);

            var result21 = Math.DivRem(0, right);
            var result22 = Math.DivRem(left, 2);

            int.DivRem(0, 10);
            int.DivRem(left, 1);

            Math.DivRem(0, 10);
            Math.DivRem(left, 1);
        }
    }
}