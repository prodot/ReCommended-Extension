using System;
using System.Globalization;

namespace Test
{
    public class UInt128s
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = UInt128.TryParse(s, NumberStyles.Integer, provider, out _);
            var result12 = UInt128.TryParse(s, null, out _);

            var result21 = UInt128.TryParse(s1, NumberStyles.Integer, provider, out _);
            var result22 = UInt128.TryParse(s1, null, out _);

            var result31 = UInt128.TryParse(utf8Text, NumberStyles.Integer, provider, out _);
            var result32 = UInt128.TryParse(utf8Text, null, out _);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = UInt128.TryParse(s, style, provider, out _);

            var result21 = UInt128.TryParse(s1, style, provider, out _);

            var result31 = UInt128.TryParse(utf8Text, style, provider, out _);
        }
    }
}