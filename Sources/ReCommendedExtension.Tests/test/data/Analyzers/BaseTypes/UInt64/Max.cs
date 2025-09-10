using System;

namespace Test
{
    public class UInt64s
    {
        public void ExpressionResult(ulong number)
        {
            var result1 = ulong.Max(10, 10);

            var result2 = Math.Max(10ul, 10ul);
        }

        public void NoDetection(ulong x, ulong y)
        {
            var result11 = ulong.Max(1, 2);
            var result12 = ulong.Max(x, 2);
            var result13 = ulong.Max(1, y);

            ulong.Max(10, 10);

            Math.Max(10ul, 10ul);
        }
    }
}