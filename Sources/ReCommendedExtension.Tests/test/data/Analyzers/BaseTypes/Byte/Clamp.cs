using System;

namespace Test
{
    public class Bytes
    {
        public void ExpressionResult(byte number)
        {
            var result11 = byte.Clamp(number, 1, 1);
            var result12 = byte.Clamp(number, byte.MinValue, byte.MaxValue);

            var result21 = Math.Clamp(number, (byte)1, (byte)1);
            var result22 = Math.Clamp(number, byte.MinValue, byte.MaxValue);
        }

        public void NoDetection(byte number, byte min, byte max)
        {
            var result11 = byte.Clamp(number, 1, max);
            var result12 = byte.Clamp(number, min, 10);

            var result21 = Math.Clamp(number, (byte)1, max);
            var result22 = Math.Clamp(number, min, byte.MaxValue);

            byte.Clamp(number, 1, 1);
            byte.Clamp(number, byte.MinValue, byte.MaxValue);

            Math.Clamp(number, (byte)1, (byte)1);
            Math.Clamp(number, byte.MinValue, byte.MaxValue);
        }
    }
}