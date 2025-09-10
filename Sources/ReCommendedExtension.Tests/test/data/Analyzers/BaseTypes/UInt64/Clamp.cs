using System;

namespace Test
{
    public class UInt64s
    {
        public void ExpressionResult(ulong number)
        {
            var result11 = ulong.Clamp(number, 1, 1);
            var result12 = ulong.Clamp(number, ulong.MinValue, ulong.MaxValue);

            var result21 = Math.Clamp(number, 1ul, 1ul);
            var result22 = Math.Clamp(number, ulong.MinValue, ulong.MaxValue);
        }

        public void NoDetection(ulong number, ulong min, ulong max)
        {
            var result11 = ulong.Clamp(number, 1, max);
            var result12 = ulong.Clamp(number, min, 10);

            var result21 = Math.Clamp(number, (ulong)1, max);
            var result22 = Math.Clamp(number, min, ulong.MaxValue);

            ulong.Clamp(number, 1, 1);
            ulong.Clamp(number, ulong.MinValue, ulong.MaxValue);

            Math.Clamp(number, (ulong)1, (ulong)1);
            Math.Clamp(number, ulong.MinValue, ulong.MaxValue);
        }
    }
}