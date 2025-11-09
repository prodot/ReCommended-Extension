using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out UInt128 result)
        {
            var result11 = UInt128.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite);
            var result12 = UInt128.Parse(s, null);
            var result13 = UInt128.Parse(s, NumberStyles.Integer, null);
            var result14 = UInt128.Parse(s1, null);
            var result15 = UInt128.Parse(utf8Text, null);

            var result21 = UInt128.TryParse(s, null, out result);
            var result22 = UInt128.TryParse(s1, null, out result);
            var result23 = UInt128.TryParse(utf8Text, null, out result);
            var result24 = UInt128.TryParse(s, NumberStyles.Integer, provider, out result);
            var result25 = UInt128.TryParse(s1, NumberStyles.Integer, provider, out result);
            var result26 = UInt128.TryParse(utf8Text, NumberStyles.Integer, provider, out result);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out UInt128 result)
        {
            var result11 = UInt128.Parse(s, style);
            var result12 = UInt128.Parse(s, provider);
            var result13 = UInt128.Parse(s, style, provider);
            var result14 = UInt128.Parse(s1, provider);
            var result15 = UInt128.Parse(utf8Text, provider);

            var result21 = UInt128.TryParse(s, provider, out result);
            var result22 = UInt128.TryParse(s1, provider, out result);
            var result23 = UInt128.TryParse(utf8Text, provider, out result);
            var result24 = UInt128.TryParse(s, style, provider, out result);
            var result25 = UInt128.TryParse(s1, style, provider, out result);
            var result26 = UInt128.TryParse(utf8Text, style, provider, out result);
        }
    }
}