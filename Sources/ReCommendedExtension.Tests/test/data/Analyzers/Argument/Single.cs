using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(float n, string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out float result)
        {
            var result11 = float.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowThousands);
            var result12 = float.Parse(s, null);
            var result13 = float.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, null);
            var result14 = float.Parse(s1, null);
            var result15 = float.Parse(utf8Text, null);

            var result21 = float.TryParse(s, null, out result);
            var result22 = float.TryParse(s1, null, out result);
            var result23 = float.TryParse(utf8Text, null, out result);
            var result24 = float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);
            var result25 = float.TryParse(s1, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);
            var result26 = float.TryParse(utf8Text, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

            var result31 = float.Round(n, 0);
            var result32 = float.Round(n, MidpointRounding.ToEven);
            var result33 = float.Round(n, 0, MidpointRounding.ToEven);
        }

        public void NoDetection(float n, string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, int digits, MidpointRounding rounding, out float result)
        {
            var result11 = float.Parse(s, style);
            var result12 = float.Parse(s, provider);
            var result13 = float.Parse(s, style, provider);
            var result14 = float.Parse(s1, provider);
            var result15 = float.Parse(utf8Text, provider);

            var result21 = float.TryParse(s, provider, out result);
            var result22 = float.TryParse(s1, provider, out result);
            var result23 = float.TryParse(utf8Text, provider, out result);
            var result24 = float.TryParse(s, style, provider, out result);
            var result25 = float.TryParse(s1, style, provider, out result);
            var result26 = float.TryParse(utf8Text, style, provider, out result);

            var result31 = float.Round(n, digits);
            var result32 = float.Round(n, rounding);
            var result33 = float.Round(n, digits, rounding);
        }
    }
}