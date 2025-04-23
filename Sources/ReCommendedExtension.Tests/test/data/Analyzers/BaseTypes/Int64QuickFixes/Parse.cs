using System;
using System.Globalization;

namespace Test
{
    public class Int64s
    {
        public void Parse(string s, IFormatProvider provider)
        {
            var result = long.Parse(s, NumberStyles{caret}.Integer, provider);
        }
    }
}