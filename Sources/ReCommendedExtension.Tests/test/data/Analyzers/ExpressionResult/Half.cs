using System;

namespace Test
{
    public class ExpressionResults
    {
        public void ExpressionResult(float number)
        {
            var result11 = number.Equals(null);
        }

        public void NoDetection(float number)
        {
            number.Equals(null);
        }
    }
}