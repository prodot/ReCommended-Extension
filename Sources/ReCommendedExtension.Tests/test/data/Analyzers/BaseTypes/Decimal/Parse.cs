using System;
using System.Globalization;

namespace Test
{
    public class Decimals
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = decimal.Parse(s, NumberStyles.Number);
            var result12 = decimal.Parse(s, null);
            var result13 = decimal.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, provider);
            var result14 = decimal.Parse(s, style, null);
            var result15 = decimal.Parse(s, NumberStyles.Number, null);

            var result21 = decimal.Parse(s1, null);

            var result31 = decimal.Parse(utf8Text, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = decimal.Parse(s, style);
            var result12 = decimal.Parse(s, provider);
            var result13 = decimal.Parse(s, style, provider);

            var result21 = decimal.Parse(s1, provider);

            var result31 = decimal.Parse(utf8Text, provider);
        }
    }
}