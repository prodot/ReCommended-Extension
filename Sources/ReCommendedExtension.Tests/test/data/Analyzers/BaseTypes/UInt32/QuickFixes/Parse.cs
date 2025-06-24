using System;
using System.Globalization;

namespace Test
{
    public class UInt32s
    {
        public void Parse(string s, IFormatProvider provider)
        {
            var result = uint.Parse(s, NumberStyles{caret}.Integer, provider);
        }
    }
}