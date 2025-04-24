using System;

namespace Test
{
    public class Int64s
    {
        public void ToString(long number)
        {
            var result = number.ToString(null as {caret}IFormatProvider);
        }
    }
}