using System;
using System.Globalization;

namespace Test
{
    public class Int32s
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = int.Parse(s, NumberStyles.Integer);
            var result12 = int.Parse(s, null);
            var result13 = int.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = int.Parse(s, style, null);
            var result15 = int.Parse(s, NumberStyles.Integer, null);

            var result21 = int.Parse(s1, null);

            var result31 = int.Parse(utf8Text, null);
        }

        public void TryParse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = int.TryParse(s, NumberStyles.Integer, provider, out _);
            var result12 = int.TryParse(s, null, out _);

            var result21 = int.TryParse(s1, NumberStyles.Integer, provider, out _);
            var result22 = int.TryParse(s1, null, out _);

            var result31 = int.TryParse(utf8Text, NumberStyles.Integer, provider, out _);
            var result32 = int.TryParse(utf8Text, null, out _);
        }
    }
}