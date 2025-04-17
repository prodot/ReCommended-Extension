using System;

namespace Test
{
    public class UInt16s
    {
        public void ExpressionResult(ushort left)
        {
            var result11 = ushort.DivRem(0, 10);
            var result12 = ushort.DivRem(left, 1);

            var result21 = Math.DivRem((ushort)0, (ushort)10);
            var result22 = Math.DivRem(left, (ushort)1);
        }

        public void NoDetection(ushort left, ushort right)
        {
            var result11 = ushort.DivRem(0, right);
            var result12 = ushort.DivRem(left, 2);

            var result21 = Math.DivRem((ushort)0, right);
            var result22 = Math.DivRem(left, (ushort)2);

            ushort.DivRem(0, 10);
            ushort.DivRem(left, 1);

            Math.DivRem((ushort)0, (ushort)10);
            Math.DivRem(left, (ushort)1);
        }
    }
}