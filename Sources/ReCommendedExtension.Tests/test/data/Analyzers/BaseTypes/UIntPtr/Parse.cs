using System;
using System.Globalization;

namespace Test
{
    public class UIntPtrs
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = nuint.Parse(s, NumberStyles.Integer);
            var result12 = nuint.Parse(s, null);
            var result13 = nuint.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = nuint.Parse(s, style, null);
            var result15 = nuint.Parse(s, NumberStyles.Integer, null);

            var result21 = nuint.Parse(s1, null);

            var result31 = nuint.Parse(utf8Text, null);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = nuint.Parse(s, style);
            var result12 = nuint.Parse(s, provider);
            var result13 = nuint.Parse(s, style, provider);

            var result21 = nuint.Parse(s1, provider);

            var result31 = nuint.Parse(utf8Text, provider);
        }
    }
}