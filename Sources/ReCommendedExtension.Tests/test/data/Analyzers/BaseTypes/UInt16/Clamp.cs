using System;

namespace Test
{
    public class UInt16s
    {
        public void ExpressionResult(ushort number)
        {
            var result11 = ushort.Clamp(number, 1, 1);
            var result12 = ushort.Clamp(number, ushort.MinValue, ushort.MaxValue);

            var result21 = Math.Clamp(number, (ushort)1, (ushort)1);
            var result22 = Math.Clamp(number, ushort.MinValue, ushort.MaxValue);
        }

        public void NoDetection(ushort number, ushort min, ushort max)
        {
            var result11 = ushort.Clamp(number, 1, max);
            var result12 = ushort.Clamp(number, min, 10);

            var result21 = Math.Clamp(number, (ushort)1, max);
            var result22 = Math.Clamp(number, min, ushort.MaxValue);

            ushort.Clamp(number, 1, 1);
            ushort.Clamp(number, ushort.MinValue, ushort.MaxValue);

            Math.Clamp(number, (ushort)1, (ushort)1);
            Math.Clamp(number, ushort.MinValue, ushort.MaxValue);
        }
    }
}