using System;

namespace Test
{
    public class SBytes
    {
        public void ToString(sbyte number)
        {
            var result = number.ToString(null as {caret}IFormatProvider);
        }
    }
}