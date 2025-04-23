using System;
using System.Globalization;

namespace Test
{
    public class UInt16s
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = ushort.Parse(s, NumberStyles.Integer);
            var result12 = ushort.Parse(s, null);
            var result13 = ushort.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = ushort.Parse(s, style, null);
            var result15 = ushort.Parse(s, NumberStyles.Integer, null);

            var result21 = ushort.Parse(s1, null);

            var result31 = ushort.Parse(utf8Text, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = ushort.Parse(s, style);
            var result12 = ushort.Parse(s, provider);
            var result13 = ushort.Parse(s, style, provider);

            var result21 = ushort.Parse(s1, provider);

            var result31 = ushort.Parse(utf8Text, provider);
        }
    }
}