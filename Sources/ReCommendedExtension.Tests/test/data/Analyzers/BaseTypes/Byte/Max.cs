using System;

namespace Test
{
    public class Bytes
    {
        public void ExpressionResult()
        {
            var result1 = byte.Max(10, 10);

            var result2 = Math.Max((byte)10, (byte)10);
        }

        public void NoDetection(byte x, byte y)
        {
            var result11 = byte.Max(1, 2);
            var result12 = byte.Max(x, 2);
            var result13 = byte.Max(1, y);

            byte.Max(10, 10);

            Math.Max((byte)10, (byte)10);
        }
    }
}