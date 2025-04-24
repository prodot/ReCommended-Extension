using System;
using System.Globalization;

namespace Test
{
    public class UInt64s
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = ulong.TryParse(s, NumberStyles.Integer, provider, out _);
            var result12 = ulong.TryParse(s, null, out _);

            var result21 = ulong.TryParse(s1, NumberStyles.Integer, provider, out _);
            var result22 = ulong.TryParse(s1, null, out _);

            var result31 = ulong.TryParse(utf8Text, NumberStyles.Integer, provider, out _);
            var result32 = ulong.TryParse(utf8Text, null, out _);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = ulong.TryParse(s, style, provider, out _);

            var result21 = ulong.TryParse(s1, style, provider, out _);

            var result31 = ulong.TryParse(utf8Text, style, provider, out _);
        }
    }
}