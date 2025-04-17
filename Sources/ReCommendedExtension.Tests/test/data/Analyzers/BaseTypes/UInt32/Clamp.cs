using System;

namespace Test
{
    public class UInt32s
    {
        public void ExpressionResult(uint number)
        {
            var result11 = uint.Clamp(number, 1, 1);
            var result12 = uint.Clamp(number, uint.MinValue, uint.MaxValue);

            var result21 = Math.Clamp(number, 1u, 1u);
            var result22 = Math.Clamp(number, uint.MinValue, uint.MaxValue);
        }

        public void NoDetection(uint number, uint min, uint max)
        {
            var result11 = uint.Clamp(number, 1, max);
            var result12 = uint.Clamp(number, min, 10);

            var result21 = Math.Clamp(number, (uint)1, max);
            var result22 = Math.Clamp(number, min, uint.MaxValue);

            uint.Clamp(number, 1, 1);
            uint.Clamp(number, uint.MinValue, uint.MaxValue);

            Math.Clamp(number, 1u, 1u);
            Math.Clamp(number, uint.MinValue, uint.MaxValue);
        }
    }
}