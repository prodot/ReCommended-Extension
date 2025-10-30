using System;

namespace Test
{
    public class Methods
    {
        public void BinaryOperator(UInt128 number, UInt128 value)
        {
            var result = number.Equals(value);
        }

        public void NoDetection(UInt128 number, UInt128 value)
        {
            number.Equals(value);
        }
    }
}