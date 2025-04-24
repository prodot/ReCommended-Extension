using System;

namespace Test
{
    public class UInt128s
    {
        public void ToString(UInt128 number)
        {
            var result = number.ToString(null as {caret}IFormatProvider);
        }
    }
}