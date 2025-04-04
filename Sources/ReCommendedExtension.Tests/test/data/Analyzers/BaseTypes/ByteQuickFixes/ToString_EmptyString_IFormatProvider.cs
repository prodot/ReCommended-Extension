using System;

namespace Test
{
    public class Bytes
    {
        public void ToString(byte number, IFormatProvider provider)
        {
            var result = number.ToString(""{caret}, provider);
        }
    }
}