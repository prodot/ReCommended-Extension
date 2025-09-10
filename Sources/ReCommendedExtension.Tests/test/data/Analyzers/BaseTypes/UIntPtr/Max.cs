using System;

namespace Test
{
    public class UIntPtrs
    {
        public void ExpressionResult(nuint number)
        {
            var result1 = nuint.Max(10, 10);

            var result2 = Math.Max((nuint)10, (nuint)10);
        }

        public void NoDetection(nuint x, nuint y)
        {
            var result11 = nuint.Max(1, 2);
            var result12 = nuint.Max(x, 2);
            var result13 = nuint.Max(1, y);

            nuint.Max(10, 10);

            Math.Max((nuint)10, (nuint)10);
        }
    }
}