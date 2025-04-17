using System;

namespace Test
{
    public class Bytes
    {
        public void ExpressionResult(byte left)
        {
            var result11 = byte.DivRem(0, 10);
            var result12 = byte.DivRem(left, 1);

            var result21 = Math.DivRem((byte)0, (byte)10);
            var result22 = Math.DivRem(left, (byte)1);
        }

        public void NoDetection(byte left, byte right)
        {
            var result11 = byte.DivRem(0, right);
            var result12 = byte.DivRem(left, 2);

            var result21 = Math.DivRem((byte)0, right);
            var result22 = Math.DivRem(left, (byte)2);

            byte.DivRem(0, 10);
            byte.DivRem(left, 1);

            Math.DivRem((byte)0, (byte)10);
            Math.DivRem(left, (byte)1);
        }
    }
}