using System;

namespace Test
{
    public class Decimals
    {
        public void ToString(decimal number)
        {
            var result = number.ToString(null as {caret}IFormatProvider);
        }
    }
}