using System;
using System.Globalization;

namespace Test
{
    public class UInt16s
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = ushort.TryParse(s, NumberStyles.Integer, provider, out _);
            var result12 = ushort.TryParse(s, null, out _);

            var result21 = ushort.TryParse(s1, NumberStyles.Integer, provider, out _);
            var result22 = ushort.TryParse(s1, null, out _);

            var result31 = ushort.TryParse(utf8Text, NumberStyles.Integer, provider, out _);
            var result32 = ushort.TryParse(utf8Text, null, out _);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = ushort.TryParse(s, style, provider, out _);

            var result21 = ushort.TryParse(s1, style, provider, out _);

            var result31 = ushort.TryParse(utf8Text, style, provider, out _);
        }
    }
}