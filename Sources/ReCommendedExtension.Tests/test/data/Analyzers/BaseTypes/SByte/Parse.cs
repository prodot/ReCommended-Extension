using System;
using System.Globalization;

namespace Test
{
    public class SBytes
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = sbyte.Parse(s, NumberStyles.Integer);
            var result12 = sbyte.Parse(s, null);
            var result13 = sbyte.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = sbyte.Parse(s, style, null);
            var result15 = sbyte.Parse(s, NumberStyles.Integer, null);

            var result21 = sbyte.Parse(s1, null);

            var result31 = sbyte.Parse(utf8Text, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = sbyte.Parse(s, style);
            var result12 = sbyte.Parse(s, provider);
            var result13 = sbyte.Parse(s, style, provider);

            var result21 = sbyte.Parse(s1, provider);

            var result31 = sbyte.Parse(utf8Text, provider);
        }
    }
}