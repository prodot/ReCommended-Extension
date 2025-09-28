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

        public void TryParse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = short.TryParse(s, NumberStyles.Integer, provider, out _);
            var result12 = short.TryParse(s, null, out _);

            var result21 = short.TryParse(s1, NumberStyles.Integer, provider, out _);
            var result22 = short.TryParse(s1, null, out _);

            var result31 = short.TryParse(utf8Text, NumberStyles.Integer, provider, out _);
            var result32 = short.TryParse(utf8Text, null, out _);
        }
    }
}