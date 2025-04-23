using System;
using System.Globalization;

namespace Test
{
    public class IntPtrs
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = nint.Parse(s, NumberStyles.Integer);
            var result12 = nint.Parse(s, null);
            var result13 = nint.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite, provider);
            var result14 = nint.Parse(s, style, null);
            var result15 = nint.Parse(s, NumberStyles.Integer, null);

            var result21 = nint.Parse(s1, null);

            var result31 = nint.Parse(utf8Text, null);
        }
    }
}