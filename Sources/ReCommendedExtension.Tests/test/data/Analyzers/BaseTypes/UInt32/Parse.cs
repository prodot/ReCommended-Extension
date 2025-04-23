using System;
using System.Globalization;

namespace Test
{
    public class UInt32s
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = uint.Parse(s, NumberStyles.Integer);
            var result12 = uint.Parse(s, null);
            var result13 = uint.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = uint.Parse(s, style, null);
            var result15 = uint.Parse(s, NumberStyles.Integer, null);

            var result21 = uint.Parse(s1, null);

            var result31 = uint.Parse(utf8Text, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = uint.Parse(s, style);
            var result12 = uint.Parse(s, provider);
            var result13 = uint.Parse(s, style, provider);

            var result21 = uint.Parse(s1, provider);

            var result31 = uint.Parse(utf8Text, provider);
        }
    }
}