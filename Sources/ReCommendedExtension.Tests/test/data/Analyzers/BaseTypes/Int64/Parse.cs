using System;
using System.Globalization;

namespace Test
{
    public class Int64s
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = long.Parse(s, NumberStyles.Integer);
            var result12 = long.Parse(s, null);
            var result13 = long.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = long.Parse(s, style, null);
            var result15 = long.Parse(s, NumberStyles.Integer, null);

            var result21 = long.Parse(s1, null);

            var result31 = long.Parse(utf8Text, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = long.Parse(s, style);
            var result12 = long.Parse(s, provider);
            var result13 = long.Parse(s, style, provider);

            var result21 = long.Parse(s1, provider);

            var result31 = long.Parse(utf8Text, provider);
        }
    }
}