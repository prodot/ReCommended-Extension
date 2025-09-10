using System;

namespace Test
{
    public class SBytes
    {
        public void ExpressionResult(sbyte number)
        {
            var result1 = sbyte.Min(10, 10);

            var result2 = Math.Min((sbyte)10, (sbyte)10);
        }

        public void NoDetection(sbyte x, sbyte y)
        {
            var result11 = sbyte.Min(1, 2);
            var result12 = sbyte.Min(x, 2);
            var result13 = sbyte.Min(1, y);

            sbyte.Min(10, 10);

            Math.Min((sbyte)10, (sbyte)10);
        }
    }
}