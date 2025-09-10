using System;
using System.Globalization;

namespace Test
{
    public class Int32s
    {
        public void Parse(string s, IFormatProvider provider)
        {
            var result = int.Parse(s, NumberStyles{caret}.Integer, provider);
        }
    }
}