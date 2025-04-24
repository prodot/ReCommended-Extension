using System;

namespace Test
{
    public class Int128s
    {
        public void Equals(Int128 number, Int128 obj)
        {
            var result = number.Equals(obj);
        }

        public void IsNegative(Int128 number)
        {
            var result = Int128.IsNegative(number);
        }

        public void IsPositive(Int128 number)
        {
            var result = Int128.IsPositive(number);
        }
    }
}