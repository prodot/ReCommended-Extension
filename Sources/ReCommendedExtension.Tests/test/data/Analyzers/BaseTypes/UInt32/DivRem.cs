using System;

namespace Test
{
    public class UInt32s
    {
        public void ExpressionResult(uint left)
        {
            var result11 = uint.DivRem(0, 10);

            var result21 = Math.DivRem(0u, 10u);
        }

        public void NoDetection(uint left, uint right)
        {
            var result11 = uint.DivRem(0, right);

            var result21 = Math.DivRem(0u, right);

            uint.DivRem(0, 10);

            Math.DivRem(0u, 10u);
        }
    }
}