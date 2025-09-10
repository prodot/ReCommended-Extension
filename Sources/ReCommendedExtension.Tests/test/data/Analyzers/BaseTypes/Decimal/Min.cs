using System;

namespace Test
{
    public class Decimals
    {
        public void ExpressionResult(decimal number)
        {
            var result1 = decimal.Min(10, 10);

            var result2 = Math.Min(10m, 10m);
        }

        public void NoDetection(decimal x, decimal y)
        {
            var result11 = decimal.Min(1, 2);
            var result12 = decimal.Min(x, 2);
            var result13 = decimal.Min(1, y);

            decimal.Min(10, 10);

            Math.Min(10m, 10m);
        }
    }
}