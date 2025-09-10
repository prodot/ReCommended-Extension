using System;

namespace Test
{
    public class UInt128s
    {
        public void ExpressionResult(UInt128 number)
        {
            var result = UInt128.Clamp(number, 1, 1);
        }

        public void NoDetection(UInt128 number, UInt128 min, UInt128 max)
        {
            var result11 = UInt128.Clamp(number, 1, max);
            var result12 = UInt128.Clamp(number, min, 10);

            var result21 = UInt128.Clamp(number, UInt128.MinValue, UInt128.MaxValue);

            UInt128.Clamp(number, 1, 1);
        }
    }
}