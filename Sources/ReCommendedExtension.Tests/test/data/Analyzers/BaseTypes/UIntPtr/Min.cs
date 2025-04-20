using System;

namespace Test
{
    public class UIntPtrs
    {
        public void ExpressionResult(nuint number)
        {
            var result1 = nuint.Min(10, 10);

            var result2 = Math.Min((nuint)10, (nuint)10);
        }

        public void NoDetection(nuint x, nuint y)
        {
            var result11 = nuint.Min(1, 2);
            var result12 = nuint.Min(x, 2);
            var result13 = nuint.Min(1, y);

            nuint.Min(10, 10);

            Math.Min((nuint)10, (nuint)10);
        }
    }
}