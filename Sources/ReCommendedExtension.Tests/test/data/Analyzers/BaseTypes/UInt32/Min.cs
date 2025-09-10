using System;

namespace Test
{
    public class UInt32s
    {
        public void ExpressionResult(uint number)
        {
            var result1 = uint.Min(10, 10);

            var result2 = Math.Min(10u, 10u);
        }

        public void NoDetection(uint x, uint y)
        {
            var result11 = uint.Min(1, 2);
            var result12 = uint.Min(x, 2);
            var result13 = uint.Min(1, y);

            uint.Min(10, 10);

            Math.Min(10u, 10u);
        }
    }
}