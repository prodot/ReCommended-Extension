using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out sbyte result)
        {
            var result11 = sbyte.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite);
            var result12 = sbyte.Parse(s, null);
            var result13 = sbyte.Parse(s, NumberStyles.Integer, null);
            var result14 = sbyte.Parse(s1, null);
            var result15 = sbyte.Parse(utf8Text, null);

            var result21 = sbyte.TryParse(s, null, out result);
            var result22 = sbyte.TryParse(s1, null, out result);
            var result23 = sbyte.TryParse(utf8Text, null, out result);
            var result24 = sbyte.TryParse(s, NumberStyles.Integer, provider, out result);
            var result25 = sbyte.TryParse(s1, NumberStyles.Integer, provider, out result);
            var result26 = sbyte.TryParse(utf8Text, NumberStyles.Integer, provider, out result);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out sbyte result)
        {
            var result11 = sbyte.Parse(s, style);
            var result12 = sbyte.Parse(s, provider);
            var result13 = sbyte.Parse(s, style, provider);
            var result14 = sbyte.Parse(s1, provider);
            var result15 = sbyte.Parse(utf8Text, provider);

            var result21 = sbyte.TryParse(s, provider, out result);
            var result22 = sbyte.TryParse(s1, provider, out result);
            var result23 = sbyte.TryParse(utf8Text, provider, out result);
            var result24 = sbyte.TryParse(s, style, provider, out result);
            var result25 = sbyte.TryParse(s1, style, provider, out result);
            var result26 = sbyte.TryParse(utf8Text, style, provider, out result);
        }
    }
}