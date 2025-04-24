using System;
using System.Globalization;

namespace Test
{
    public class UInt16s
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = ushort.Parse(s, NumberStyles.Integer);
            var result12 = ushort.Parse(s, null);
            var result13 = ushort.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = ushort.Parse(s, style, null);
            var result15 = ushort.Parse(s, NumberStyles.Integer, null);

            var result21 = ushort.Parse(s1, null);

            var result31 = ushort.Parse(utf8Text, null);
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
            var result11 = ushort.TryParse(s, NumberStyles.Integer, provider, out _);
            var result12 = ushort.TryParse(s, null, out _);

            var result21 = ushort.TryParse(s1, NumberStyles.Integer, provider, out _);
            var result22 = ushort.TryParse(s1, null, out _);

            var result31 = ushort.TryParse(utf8Text, NumberStyles.Integer, provider, out _);
            var result32 = ushort.TryParse(utf8Text, null, out _);
        }
    }
}