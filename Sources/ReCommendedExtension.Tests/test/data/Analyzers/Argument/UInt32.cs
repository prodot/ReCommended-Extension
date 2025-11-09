using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out uint result)
        {
            var result11 = uint.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite);
            var result12 = uint.Parse(s, null);
            var result13 = uint.Parse(s, NumberStyles.Integer, null);
            var result14 = uint.Parse(s1, null);
            var result15 = uint.Parse(utf8Text, null);

            var result21 = uint.TryParse(s, null, out result);
            var result22 = uint.TryParse(s1, null, out result);
            var result23 = uint.TryParse(utf8Text, null, out result);
            var result24 = uint.TryParse(s, NumberStyles.Integer, provider, out result);
            var result25 = uint.TryParse(s1, NumberStyles.Integer, provider, out result);
            var result26 = uint.TryParse(utf8Text, NumberStyles.Integer, provider, out result);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider, out uint result)
        {
            var result11 = uint.Parse(s, style);
            var result12 = uint.Parse(s, provider);
            var result13 = uint.Parse(s, style, provider);
            var result14 = uint.Parse(s1, provider);
            var result15 = uint.Parse(utf8Text, provider);

            var result21 = uint.TryParse(s, provider, out result);
            var result22 = uint.TryParse(s1, provider, out result);
            var result23 = uint.TryParse(utf8Text, provider, out result);
            var result24 = uint.TryParse(s, style, provider, out result);
            var result25 = uint.TryParse(s1, style, provider, out result);
            var result26 = uint.TryParse(utf8Text, style, provider, out result);
        }
    }
}