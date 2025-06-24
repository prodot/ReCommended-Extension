using System;
using System.Globalization;

namespace Test
{
    public class SBytes
    {
        public void Parse(string s, IFormatProvider provider)
        {
            var result = sbyte.Parse(s, NumberStyles{caret}.Integer, provider);
        }
    }
}