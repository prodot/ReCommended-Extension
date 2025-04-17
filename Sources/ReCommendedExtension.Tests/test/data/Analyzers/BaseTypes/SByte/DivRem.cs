using System;

namespace Test
{
    public class SBytes
    {
        public void ExpressionResult(sbyte left)
        {
            var result11 = sbyte.DivRem(0, 10);
            var result12 = sbyte.DivRem(left, 1);

            var result21 = Math.DivRem((sbyte)0, (sbyte)10);
            var result22 = Math.DivRem(left, (sbyte)1);
        }

        public void NoDetection(sbyte left, sbyte right)
        {
            var result11 = sbyte.DivRem(0, right);
            var result12 = sbyte.DivRem(left, 2);

            var result21 = Math.DivRem((sbyte)0, right);
            var result22 = Math.DivRem(left, (sbyte)2);

            sbyte.DivRem(0, 10);
            sbyte.DivRem(left, 1);

            Math.DivRem((sbyte)0, (sbyte)10);
            Math.DivRem(left, (sbyte)1);
        }
    }
}