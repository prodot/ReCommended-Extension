using System;
using System.Globalization;

namespace Test
{
    public class Bytes
    {
        public void TryParse(string s, IFormatProvider provider)
        {
            var result = byte.TryParse(s, Number{caret}Styles.Integer, provider, out _);
        }
    }
}