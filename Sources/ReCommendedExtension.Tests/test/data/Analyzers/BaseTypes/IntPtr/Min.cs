using System;

namespace Test
{
    public class IntPtrs
    {
        public void ExpressionResult(nint number)
        {
            var result1 = nint.Min(10, 10);

            var result2 = Math.Min((nint)10, (nint)10);
        }

        public void NoDetection(nint x, nint y)
        {
            var result11 = nint.Min(1, 2);
            var result12 = nint.Min(x, 2);
            var result13 = nint.Min(1, y);

            nint.Min(10, 10);

            Math.Min((nint)10, (nint)10);
        }
    }
}