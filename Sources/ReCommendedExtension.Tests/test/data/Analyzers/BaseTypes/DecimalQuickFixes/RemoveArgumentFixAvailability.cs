using System;
using System.Globalization;

namespace Test
{
    public class Decimals
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = decimal.Parse(s, NumberStyles.Number);
            var result12 = decimal.Parse(s, null);
            var result13 = decimal.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider);
            var result14 = decimal.Parse(s, style, null);
            var result15 = decimal.Parse(s, NumberStyles.Number, null);

            var result21 = decimal.Parse(s1, null);

            var result31 = decimal.Parse(utf8Text, null);
        }

        public void Round(decimal d, int decimals, MidpointRounding mode)
        {
            var result11 = decimal.Round(d, 0);
            var result12 = decimal.Round(d, 0, mode);
            var result13 = decimal.Round(d, MidpointRounding.ToEven);
            var result14 = decimal.Round(d, decimals, MidpointRounding.ToEven);
            var result15 = decimal.Round(d, 0, MidpointRounding.ToEven);

            var result21 = Math.Round(d, 0);
            var result22 = Math.Round(d, 0, mode);
            var result23 = Math.Round(d, MidpointRounding.ToEven);
            var result24 = Math.Round(d, decimals, MidpointRounding.ToEven);
            var result25 = Math.Round(d, 0, MidpointRounding.ToEven);
        }

        public void ToString(decimal number, string format, IFormatProvider provider)
        {
            var result11 = number.ToString(null as string);
            var result12 = number.ToString("");

            var result21 = number.ToString("G");
            var result22 = number.ToString("G0");
            var result23 = number.ToString("G29");
            var result24 = number.ToString("g");
            var result25 = number.ToString("g0");
            var result26 = number.ToString("g29");

            var result31 = number.ToString(null as IFormatProvider);
            var result32 = number.ToString(null, provider);
            var result33 = number.ToString("", provider);
            var result34 = number.ToString(format, null);
            var result35 = number.ToString("", null);

            var result41 = number.ToString("G", provider);
            var result42 = number.ToString("G0", provider);
            var result43 = number.ToString("G29", provider);
            var result44 = number.ToString("g", provider);
            var result45 = number.ToString("g0", provider);
            var result46 = number.ToString("g29", provider);
        }

        public void TryParse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = decimal.TryParse(s, NumberStyles.Number, provider, out _);
            var result12 = decimal.TryParse(s, null, out _);

            var result21 = decimal.TryParse(s1, NumberStyles.Number, provider, out _);
            var result22 = decimal.TryParse(s1, null, out _);

            var result31 = decimal.TryParse(utf8Text, NumberStyles.Number, provider, out _);
            var result32 = decimal.TryParse(utf8Text, null, out _);
        }
    }
}