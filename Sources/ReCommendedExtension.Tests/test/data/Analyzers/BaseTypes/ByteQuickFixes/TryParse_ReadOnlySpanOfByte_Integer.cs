using System;
using System.Globalization;

namespace Test
{
    public class Bytes
    {
        public void TryParse(ReadOnlySpan<byte> utf8Text, IFormatProvider provider)
        {
            var result = byte.TryParse(utf8Text, Number{caret}Styles.Integer, provider, out _);
        }
    }
}