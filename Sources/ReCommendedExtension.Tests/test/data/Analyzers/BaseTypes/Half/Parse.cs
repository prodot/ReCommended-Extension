using System;
using System.Globalization;

namespace Test
{
    public class Halves
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = Half.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands);
            var result12 = Half.Parse(s, null);
            var result13 = Half.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowThousands, provider);
            var result14 = Half.Parse(s, style, null);
            var result15 = Half.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, null);

            var result21 = Half.Parse(s1, null);

            var result31 = Half.Parse(utf8Text, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = Half.Parse(s, style);
            var result12 = Half.Parse(s, provider);
            var result13 = Half.Parse(s, style, provider);

            var result21 = Half.Parse(s1, provider);

            var result31 = Half.Parse(utf8Text, provider);
        }
    }
}