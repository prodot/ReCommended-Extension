using System;

namespace Test
{
    public class SBytes
    {
        public void ExpressionResult(sbyte number)
        {
            var result1 = sbyte.Max(10, 10);

            var result2 = Math.Max((sbyte)10, (sbyte)10);
        }

        public void NoDetection(sbyte x, sbyte y)
        {
            var result11 = sbyte.Max(1, 2);
            var result12 = sbyte.Max(x, 2);
            var result13 = sbyte.Max(1, y);

            sbyte.Max(10, 10);

            Math.Max((sbyte)10, (sbyte)10);
        }
    }
}