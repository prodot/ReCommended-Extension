using System;

namespace Test
{
    public class Int128s
    {
        public void Operator(Int128 number)
        {
            var result = Int128.IsPositive(number);
        }

        public void NoDetection(Int128 number)
        {
            Int128.IsPositive(number);
        }
    }
}