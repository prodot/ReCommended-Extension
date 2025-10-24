using System;

namespace Test
{
    public class Methods
    {
        public void BinaryOperator(Int128 number, Int128 value)
        {
            var result11 = number.Equals(value);

            var result21 = Int128.IsNegative(number);
            var result22 = Int128.IsPositive(number);
        }

        public void NoDetection(Int128 number, Int128 value)
        {
            number.Equals(value);

            Int128.IsNegative(number);
            Int128.IsPositive(number);
        }
    }
}