using System;

namespace Test
{
    public class UInt64s
    {
        public void ExpressionResult(ulong number)
        {
            var result1 = ulong.Min(10, 10);

            var result2 = Math.Min(10ul, 10ul);
        }

        public void NoDetection(ulong x, ulong y)
        {
            var result11 = ulong.Min(1, 2);
            var result12 = ulong.Min(x, 2);
            var result13 = ulong.Min(1, y);

            ulong.Min(10, 10);

            Math.Min(10ul, 10ul);
        }
    }
}