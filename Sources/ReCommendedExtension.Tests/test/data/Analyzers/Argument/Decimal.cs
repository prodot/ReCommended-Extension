using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(decimal n, string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out decimal result)
        {
            var result11 = decimal.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            var result12 = decimal.Parse(s, null);
            var result13 = decimal.Parse(s, NumberStyles.Number, null);
            var result14 = decimal.Parse(s1, null);
            var result15 = decimal.Parse(utf8Text, null);

            var result21 = decimal.TryParse(s, null, out result);
            var result22 = decimal.TryParse(s1, null, out result);
            var result23 = decimal.TryParse(utf8Text, null, out result);
            var result24 = decimal.TryParse(s, NumberStyles.Number, provider, out result);
            var result25 = decimal.TryParse(s1, NumberStyles.Number, provider, out result);
            var result26 = decimal.TryParse(utf8Text, NumberStyles.Number, provider, out result);

            var result31 = decimal.Round(n, 0);
            var result32 = decimal.Round(n, MidpointRounding.ToEven);
            var result33 = decimal.Round(n, 0, MidpointRounding.ToEven);
        }

        public void NoDetection(decimal n, string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, int decimals, MidpointRounding rounding, out decimal result)
        {
            var result11 = decimal.Parse(s, style);
            var result12 = decimal.Parse(s, provider);
            var result13 = decimal.Parse(s, style, provider);
            var result14 = decimal.Parse(s1, provider);
            var result15 = decimal.Parse(utf8Text, provider);

            var result21 = decimal.TryParse(s, provider, out result);
            var result22 = decimal.TryParse(s1, provider, out result);
            var result23 = decimal.TryParse(utf8Text, provider, out result);
            var result24 = decimal.TryParse(s, style, provider, out result);
            var result25 = decimal.TryParse(s1, style, provider, out result);
            var result26 = decimal.TryParse(utf8Text, style, provider, out result);

            var result31 = decimal.Round(n, decimals);
            var result32 = decimal.Round(n, rounding);
            var result33 = decimal.Round(n, decimals, rounding);
        }
    }
}