using System;

namespace Test
{
    public class Int128s
    {
        public void IsNegative(Int128 number)
        {
            var result = !(Int128.Is{caret}Negative(number));
        }
    }
}