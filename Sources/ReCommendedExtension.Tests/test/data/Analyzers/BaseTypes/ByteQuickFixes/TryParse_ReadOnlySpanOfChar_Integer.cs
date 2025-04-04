using System;
using System.Globalization;

namespace Test
{
    public class Bytes
    {
        public void TryParse(ReadOnlySpan<char> s, IFormatProvider provider)
        {
            var result = byte.TryParse(s, Number{caret}Styles.Integer, provider, out _);
        }
    }
}