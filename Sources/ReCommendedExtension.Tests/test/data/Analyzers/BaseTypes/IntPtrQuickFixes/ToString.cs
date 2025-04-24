using System;

namespace Test
{
    public class IntPtrs
    {
        public void ToString(nint number)
        {
            var result = number.ToString(null as {caret}IFormatProvider);
        }
    }
}