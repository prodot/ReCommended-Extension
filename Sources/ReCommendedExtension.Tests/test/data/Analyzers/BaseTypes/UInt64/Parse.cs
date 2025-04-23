using System;
using System.Globalization;

namespace Test
{
    public class UInt64s
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = ulong.Parse(s, NumberStyles.Integer);
            var result12 = ulong.Parse(s, null);
            var result13 = ulong.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = ulong.Parse(s, style, null);
            var result15 = ulong.Parse(s, NumberStyles.Integer, null);

            var result21 = ulong.Parse(s1, null);

            var result31 = ulong.Parse(utf8Text, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = ulong.Parse(s, style);
            var result12 = ulong.Parse(s, provider);
            var result13 = ulong.Parse(s, style, provider);

            var result21 = ulong.Parse(s1, provider);

            var result31 = ulong.Parse(utf8Text, provider);
        }
    }
}