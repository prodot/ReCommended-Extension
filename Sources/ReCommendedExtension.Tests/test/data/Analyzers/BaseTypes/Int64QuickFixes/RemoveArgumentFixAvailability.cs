using System;
using System.Globalization;

namespace Test
{
    public class Int64s
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = long.Parse(s, NumberStyles.Integer);
            var result12 = long.Parse(s, null);
            var result13 = long.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = long.Parse(s, style, null);
            var result15 = long.Parse(s, NumberStyles.Integer, null);

            var result21 = long.Parse(s1, null);

            var result31 = long.Parse(utf8Text, null);
        }

        public void ToString(byte number, string format, IFormatProvider provider)
        {
            var result1 = number.ToString(null as string);
            var result2 = number.ToString("");
            var result3 = number.ToString(null as IFormatProvider);
            var result4 = number.ToString(null, provider);
            var result5 = number.ToString("", provider);
            var result6 = number.ToString(format, null);
            var result7 = number.ToString("", null);
        }

        public void TryParse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = long.TryParse(s, NumberStyles.Integer, provider, out _);
            var result12 = long.TryParse(s, null, out _);

            var result21 = long.TryParse(s1, NumberStyles.Integer, provider, out _);
            var result22 = long.TryParse(s1, null, out _);

            var result31 = long.TryParse(utf8Text, NumberStyles.Integer, provider, out _);
            var result32 = long.TryParse(utf8Text, null, out _);
        }
    }
}