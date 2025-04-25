using System;

namespace Test
{
    public class Int128s
    {
        public void ExpressionResult(Int128 number)
        {
            var result = Int128.MinMagnitude(10, 10);
        }

        public void NoDetection(Int128 x, Int128 y)
        {
            var result11 = Int128.MinMagnitude(1, 2);
            var result12 = Int128.MinMagnitude(x, 2);
            var result13 = Int128.MinMagnitude(1, y);

            Int128.MinMagnitude(10, 10);
        }
    }
}