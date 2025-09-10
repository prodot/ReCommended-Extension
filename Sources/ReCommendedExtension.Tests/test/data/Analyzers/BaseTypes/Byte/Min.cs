using System;

namespace Test
{
    public class Bytes
    {
        public void ExpressionResult()
        {
            var result1 = byte.Min(10, 10);

            var result2 = Math.Min((byte)10, (byte)10);
        }

        public void NoDetection(byte x, byte y)
        {
            var result11 = byte.Min(1, 2);
            var result12 = byte.Min(x, 2);
            var result13 = byte.Min(1, y);

            byte.Min(10, 10);

            Math.Min((byte)10, (byte)10);
        }
    }
}