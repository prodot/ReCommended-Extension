using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(Half n, string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out Half result)
        {
            var result11 = Half.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowThousands);
            var result12 = Half.Parse(s, null);
            var result13 = Half.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, null);
            var result14 = Half.Parse(s1, null);
            var result15 = Half.Parse(utf8Text, null);

            var result21 = Half.TryParse(s, null, out result);
            var result22 = Half.TryParse(s1, null, out result);
            var result23 = Half.TryParse(utf8Text, null, out result);
            var result24 = Half.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);
            var result25 = Half.TryParse(s1, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);
            var result26 = Half.TryParse(utf8Text, NumberStyles.Float | NumberStyles.AllowThousands, provider, out result);

            var result31 = Half.Round(n, 0);
            var result32 = Half.Round(n, MidpointRounding.ToEven);
            var result33 = Half.Round(n, 0, MidpointRounding.ToEven);
        }

        public void NoDetection(Half n, string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, int digits, MidpointRounding rounding, out Half result)
        {
            var result11 = Half.Parse(s, style);
            var result12 = Half.Parse(s, provider);
            var result13 = Half.Parse(s, style, provider);
            var result14 = Half.Parse(s1, provider);
            var result15 = Half.Parse(utf8Text, provider);

            var result21 = Half.TryParse(s, provider, out result);
            var result22 = Half.TryParse(s1, provider, out result);
            var result23 = Half.TryParse(utf8Text, provider, out result);
            var result24 = Half.TryParse(s, style, provider, out result);
            var result25 = Half.TryParse(s1, style, provider, out result);
            var result26 = Half.TryParse(utf8Text, style, provider, out result);

            var result31 = Half.Round(n, digits);
            var result32 = Half.Round(n, rounding);
            var result33 = Half.Round(n, digits, rounding);
        }
    }
}