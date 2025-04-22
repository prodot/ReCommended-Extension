using System;

namespace Test
{
    public class SBytes
    {
        public void ExpressionResult(sbyte left)
        {
            var result11 = sbyte.DivRem(0, 10);

            var result21 = Math.DivRem((sbyte)0, (sbyte)10);
        }

        public void NoDetection(sbyte left, sbyte right)
        {
            var result11 = sbyte.DivRem(0, right);

            var result21 = Math.DivRem((sbyte)0, right);

            sbyte.DivRem(0, 10);

            Math.DivRem((sbyte)0, (sbyte)10);
        }
    }
}