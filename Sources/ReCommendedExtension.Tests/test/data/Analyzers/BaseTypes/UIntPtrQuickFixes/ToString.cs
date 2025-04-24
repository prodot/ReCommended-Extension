using System;

namespace Test
{
    public class UIntPtrs
    {
        public void ToString(nuint number)
        {
            var result = number.ToString(null as {caret}IFormatProvider);
        }
    }
}