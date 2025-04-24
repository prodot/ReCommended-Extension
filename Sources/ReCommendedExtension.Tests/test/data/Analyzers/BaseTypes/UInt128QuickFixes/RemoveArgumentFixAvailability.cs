using System;
using System.Globalization;

namespace Test
{
    public class UInt128s
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = UInt128.Parse(s, NumberStyles.Integer);
            var result12 = UInt128.Parse(s, null);
            var result13 = UInt128.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = UInt128.Parse(s, style, null);
            var result15 = UInt128.Parse(s, NumberStyles.Integer, null);

            var result21 = UInt128.Parse(s1, null);

            var result31 = UInt128.Parse(utf8Text, null);
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
            var result11 = UInt128.TryParse(s, NumberStyles.Integer, provider, out _);
            var result12 = UInt128.TryParse(s, null, out _);

            var result21 = UInt128.TryParse(s1, NumberStyles.Integer, provider, out _);
            var result22 = UInt128.TryParse(s1, null, out _);

            var result31 = UInt128.TryParse(utf8Text, NumberStyles.Integer, provider, out _);
            var result32 = UInt128.TryParse(utf8Text, null, out _);
        }
    }
}