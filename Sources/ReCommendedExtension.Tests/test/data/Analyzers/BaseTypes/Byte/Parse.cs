using System;
using System.Globalization;

namespace Test
{
    public class Bytes
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = byte.Parse(s, NumberStyles.Integer);
            var result12 = byte.Parse(s, null);
            var result13 = byte.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = byte.Parse(s, style, null);
            var result15 = byte.Parse(s, NumberStyles.Integer, null);

            var result21 = byte.Parse(s1, null);

            var result31 = byte.Parse(utf8Text, null);
        }

        public void NoDetection()
        {
            
        }
    }
}