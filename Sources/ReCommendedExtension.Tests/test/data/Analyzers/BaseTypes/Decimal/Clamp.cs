using System;

namespace Test
{
    public class Decimals
    {
        public void ExpressionResult(decimal number)
        {
            var result11 = decimal.Clamp(number, 1, 1);
            var result12 = decimal.Clamp(number, decimal.MinValue, decimal.MaxValue);

            var result21 = Math.Clamp(number, 1m, 1m);
            var result22 = Math.Clamp(number, decimal.MinValue, decimal.MaxValue);
        }

        public void NoDetection(decimal number, decimal min, decimal max)
        {
            var result11 = decimal.Clamp(number, 1, max);
            var result12 = decimal.Clamp(number, min, 10);

            var result21 = Math.Clamp(number, 1m, max);
            var result22 = Math.Clamp(number, min, decimal.MaxValue);

            decimal.Clamp(number, 1, 1);
            decimal.Clamp(number, decimal.MinValue, decimal.MaxValue);

            Math.Clamp(number, 1m, 1m);
            Math.Clamp(number, decimal.MinValue, decimal.MaxValue);
        }
    }
}