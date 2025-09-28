using System;
using System.Globalization;

namespace Test
{
    public class UIntPtrs
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = nuint.Parse(s, NumberStyles.Integer);
            var result12 = nuint.Parse(s, null);
            var result13 = nuint.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = nuint.Parse(s, style, null);
            var result15 = nuint.Parse(s, NumberStyles.Integer, null);

            var result21 = nuint.Parse(s1, null);

            var result31 = nuint.Parse(utf8Text, null);
        }

        public void TryParse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = nuint.TryParse(s, NumberStyles.Integer, provider, out _);
            var result12 = nuint.TryParse(s, null, out _);

            var result21 = nuint.TryParse(s1, NumberStyles.Integer, provider, out _);
            var result22 = nuint.TryParse(s1, null, out _);

            var result31 = nuint.TryParse(utf8Text, NumberStyles.Integer, provider, out _);
            var result32 = nuint.TryParse(utf8Text, null, out _);
        }
    }
}