using System;
using System.Globalization;

namespace Test
{
    public class Decimals
    {
        public void Parse(string s, IFormatProvider provider)
        {
            var result = decimal.Parse(s, NumberStyles{caret}.Number, provider);
        }
    }
}