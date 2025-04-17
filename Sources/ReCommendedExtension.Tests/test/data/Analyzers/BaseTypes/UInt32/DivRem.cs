using System;

namespace Test
{
    public class UInt32s
    {
        public void ExpressionResult(uint left)
        {
            var result11 = uint.DivRem(0, 10);
            var result12 = uint.DivRem(left, 1);

            var result21 = Math.DivRem(0u, 10u);
            var result22 = Math.DivRem(left, 1u);
        }

        public void NoDetection(uint left, uint right)
        {
            var result11 = uint.DivRem(0, right);
            var result12 = uint.DivRem(left, 2);

            var result21 = Math.DivRem(0u, right);
            var result22 = Math.DivRem(left, 2u);

            uint.DivRem(0, 10);
            uint.DivRem(left, 1);

            Math.DivRem(0u, 10u);
            Math.DivRem(left, 1u);
        }
    }
}