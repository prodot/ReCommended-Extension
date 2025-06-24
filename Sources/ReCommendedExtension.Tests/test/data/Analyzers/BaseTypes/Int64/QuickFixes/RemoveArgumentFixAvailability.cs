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

        public void ToString(long number, string format, IFormatProvider provider)
        {
            var result11 = number.ToString(null as string);
            var result12 = number.ToString("");

            var result21 = number.ToString("G");
            var result22 = number.ToString("G0");
            var result23 = number.ToString("G19");
            var result24 = number.ToString("g");
            var result25 = number.ToString("g0");
            var result26 = number.ToString("g19");

            var result31 = number.ToString(null as IFormatProvider);
            var result32 = number.ToString(null, provider);
            var result33 = number.ToString("", provider);
            var result34 = number.ToString(format, null);
            var result35 = number.ToString("", null);

            var result41 = number.ToString("G", provider);
            var result42 = number.ToString("G0", provider);
            var result43 = number.ToString("G19", provider);
            var result44 = number.ToString("g", provider);
            var result45 = number.ToString("g0", provider);
            var result46 = number.ToString("g19", provider);
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