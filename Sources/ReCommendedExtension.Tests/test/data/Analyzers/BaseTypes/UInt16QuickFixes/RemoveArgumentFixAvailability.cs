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

        public void ToString(ushort number, string format, IFormatProvider provider)
        {
            var result11 = number.ToString(null as string);
            var result12 = number.ToString("");

            var result21 = number.ToString("G");
            var result22 = number.ToString("G0");
            var result23 = number.ToString("G5");
            var result24 = number.ToString("g");
            var result25 = number.ToString("g0");
            var result26 = number.ToString("g5");

            var result31 = number.ToString(null as IFormatProvider);
            var result32 = number.ToString(null, provider);
            var result33 = number.ToString("", provider);
            var result34 = number.ToString(format, null);
            var result35 = number.ToString("", null);

            var result41 = number.ToString("G", provider);
            var result42 = number.ToString("G0", provider);
            var result43 = number.ToString("G5", provider);
            var result44 = number.ToString("g", provider);
            var result45 = number.ToString("g0", provider);
            var result46 = number.ToString("g5", provider);
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