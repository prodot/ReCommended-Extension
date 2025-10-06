using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out long result)
        {
            var result11 = long.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite);
            var result12 = long.Parse(s, null);
            var result13 = long.Parse(s, NumberStyles.Integer, null);
            var result14 = long.Parse(s1, null);
            var result15 = long.Parse(utf8Text, null);

            var result21 = long.TryParse(s, null, out result);
            var result22 = long.TryParse(s1, null, out result);
            var result23 = long.TryParse(utf8Text, null, out result);
            var result24 = long.TryParse(s, NumberStyles.Integer, provider, out result);
            var result25 = long.TryParse(s1, NumberStyles.Integer, provider, out result);
            var result26 = long.TryParse(utf8Text, NumberStyles.Integer, provider, out result);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out long result)
        {
            var result11 = long.Parse(s, style);
            var result12 = long.Parse(s, provider);
            var result13 = long.Parse(s, style, provider);
            var result14 = long.Parse(s1, provider);
            var result15 = long.Parse(utf8Text, provider);

            var result21 = long.TryParse(s, provider, out result);
            var result22 = long.TryParse(s1, provider, out result);
            var result23 = long.TryParse(utf8Text, provider, out result);
            var result24 = long.TryParse(s, style, provider, out result);
            var result25 = long.TryParse(s1, style, provider, out result);
            var result26 = long.TryParse(utf8Text, style, provider, out result);
        }
    }
}