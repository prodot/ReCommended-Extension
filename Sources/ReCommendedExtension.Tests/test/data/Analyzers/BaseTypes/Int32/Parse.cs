using System;
using System.Globalization;

namespace Test
{
    public class Int32s
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = int.Parse(s, NumberStyles.Integer);
            var result12 = int.Parse(s, null);
            var result13 = int.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = int.Parse(s, style, null);
            var result15 = int.Parse(s, NumberStyles.Integer, null);

            var result21 = int.Parse(s1, null);

            var result31 = int.Parse(utf8Text, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = int.Parse(s, style);
            var result12 = int.Parse(s, provider);
            var result13 = int.Parse(s, style, provider);

            var result21 = int.Parse(s1, provider);

            var result31 = int.Parse(utf8Text, provider);
        }
    }
}