using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(double n, string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out double result)
        {
            var result11 = double.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowThousands);
            var result12 = double.Parse(s, null);
            var result13 = double.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, null);
            var result14 = double.Parse(s1, null);
            var result15 = double.Parse(utf8Text, null);

            var result21 = double.TryParse(s, null, out result);
            var result22 = double.TryParse(s1, null, out result);
            var result23 = double.TryParse(utf8Text, null, out result);
            var result24 = double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);
            var result25 = double.TryParse(s1, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);
            var result26 = double.TryParse(utf8Text, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

            var result31 = double.Round(n, 0);
            var result32 = double.Round(n, MidpointRounding.ToEven);
            var result33 = double.Round(n, 0, MidpointRounding.ToEven);
        }

        public void NoDetection(double n, string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, int digits, MidpointRounding rounding, out double result)
        {
            var result11 = double.Parse(s, style);
            var result12 = double.Parse(s, provider);
            var result13 = double.Parse(s, style, provider);
            var result14 = double.Parse(s1, provider);
            var result15 = double.Parse(utf8Text, provider);

            var result21 = double.TryParse(s, provider, out result);
            var result22 = double.TryParse(s1, provider, out result);
            var result23 = double.TryParse(utf8Text, provider, out result);
            var result24 = double.TryParse(s, style, provider, out result);
            var result25 = double.TryParse(s1, style, provider, out result);
            var result26 = double.TryParse(utf8Text, style, provider, out result);

            var result31 = double.Round(n, digits);
            var result32 = double.Round(n, rounding);
            var result33 = double.Round(n, digits, rounding);
        }
    }
}