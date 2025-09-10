using System;

namespace Test
{
    public class Bytes
    {
        public void ExpressionResult(byte left)
        {
            var result11 = byte.DivRem(0, 10);

            var result21 = Math.DivRem((byte)0, (byte)10);
        }

        public void NoDetection(byte left, byte right)
        {
            var result11 = byte.DivRem(0, right);

            var result21 = Math.DivRem((byte)0, right);

            byte.DivRem(0, 10);

            Math.DivRem((byte)0, (byte)10);
        }
    }
}