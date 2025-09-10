using System;

namespace Test
{
    public class UInt128s
    {
        public void ExpressionResult(UInt128 number)
        {
            var result = UInt128.Max(10, 10);
        }

        public void NoDetection(UInt128 x, UInt128 y)
        {
            var result11 = UInt128.Max(1, 2);
            var result12 = UInt128.Max(x, 2);
            var result13 = UInt128.Max(1, y);

            UInt128.Max(10, 10);
        }
    }
}