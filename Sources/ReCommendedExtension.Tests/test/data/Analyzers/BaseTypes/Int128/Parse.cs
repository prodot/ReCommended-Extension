using System;
using System.Globalization;

namespace Test
{
    public class Int128s
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = Int128.Parse(s, NumberStyles.Integer);
            var result12 = Int128.Parse(s, null);
            var result13 = Int128.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = Int128.Parse(s, style, null);
            var result15 = Int128.Parse(s, NumberStyles.Integer, null);

            var result21 = Int128.Parse(s1, null);

            var result31 = Int128.Parse(utf8Text, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = Int128.Parse(s, style);
            var result12 = Int128.Parse(s, provider);
            var result13 = Int128.Parse(s, style, provider);

            var result21 = Int128.Parse(s1, provider);

            var result31 = Int128.Parse(utf8Text, provider);
        }
    }
}