using System;

namespace Test
{
    public class SBytes
    {
        public void ExpressionResult(sbyte number)
        {
            var result11 = sbyte.Clamp(number, 1, 1);
            var result12 = sbyte.Clamp(number, sbyte.MinValue, sbyte.MaxValue);

            var result21 = Math.Clamp(number, (sbyte)1, (sbyte)1);
            var result22 = Math.Clamp(number, sbyte.MinValue, sbyte.MaxValue);
        }

        public void NoDetection(sbyte number, sbyte min, sbyte max)
        {
            var result11 = sbyte.Clamp(number, 1, max);
            var result12 = sbyte.Clamp(number, min, 10);

            var result21 = Math.Clamp(number, (sbyte)1, max);
            var result22 = Math.Clamp(number, min, sbyte.MaxValue);

            sbyte.Clamp(number, 1, 1);
            sbyte.Clamp(number, sbyte.MinValue, sbyte.MaxValue);

            Math.Clamp(number, (sbyte)1, (sbyte)1);
            Math.Clamp(number, sbyte.MinValue, sbyte.MaxValue);
        }
    }
}