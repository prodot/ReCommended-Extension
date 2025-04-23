using System;
using System.Globalization;

namespace Test
{
    public class Int16s
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = short.Parse(s, NumberStyles.Integer);
            var result12 = short.Parse(s, null);
            var result13 = short.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = short.Parse(s, style, null);
            var result15 = short.Parse(s, NumberStyles.Integer, null);

            var result21 = short.Parse(s1, null);

            var result31 = short.Parse(utf8Text, null);
        }
    }
}