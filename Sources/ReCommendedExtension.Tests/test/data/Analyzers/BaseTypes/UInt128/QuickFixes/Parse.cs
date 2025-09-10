using System;
using System.Globalization;

namespace Test
{
    public class UInt128s
    {
        public void Parse(string s, IFormatProvider provider)
        {
            var result = UInt128.Parse(s, NumberStyles{caret}.Integer, provider);
        }
    }
}