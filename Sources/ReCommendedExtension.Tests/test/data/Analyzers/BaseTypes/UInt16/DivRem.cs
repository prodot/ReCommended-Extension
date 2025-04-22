using System;

namespace Test
{
    public class UInt16s
    {
        public void ExpressionResult(ushort left)
        {
            var result11 = ushort.DivRem(0, 10);

            var result21 = Math.DivRem((ushort)0, (ushort)10);
        }

        public void NoDetection(ushort left, ushort right)
        {
            var result11 = ushort.DivRem(0, right);

            var result21 = Math.DivRem((ushort)0, right);

            ushort.DivRem(0, 10);

            Math.DivRem((ushort)0, (ushort)10);
        }
    }
}