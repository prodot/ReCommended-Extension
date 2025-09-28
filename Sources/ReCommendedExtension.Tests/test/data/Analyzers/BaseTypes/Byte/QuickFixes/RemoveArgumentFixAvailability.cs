using System;
using System.Globalization;

namespace Test
{
    public class Bytes
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = byte.Parse(s, NumberStyles.Integer);
            var result12 = byte.Parse(s, null);
            var result13 = byte.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = byte.Parse(s, style, null);
            var result15 = byte.Parse(s, NumberStyles.Integer, null);

            var result21 = byte.Parse(s1, null);

            var result31 = byte.Parse(utf8Text, null);
        }

        public void TryParse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = byte.TryParse(s, NumberStyles.Integer, provider, out _);
            var result12 = byte.TryParse(s, null, out _);

            var result21 = byte.TryParse(s1, NumberStyles.Integer, provider, out _);
            var result22 = byte.TryParse(s1, null, out _);

            var result31 = byte.TryParse(utf8Text, NumberStyles.Integer, provider, out _);
            var result32 = byte.TryParse(utf8Text, null, out _);
        }
    }
}