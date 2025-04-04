using System;
using System.Globalization;

namespace Test
{
    public class Bytes
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = byte.TryParse(s, NumberStyles.Integer, provider, out _);
            var result12 = byte.TryParse(s, null, out _);

            var result21 = byte.TryParse(s1, NumberStyles.Integer, provider, out _);
            var result22 = byte.TryParse(s1, null, out _);

            var result31 = byte.TryParse(utf8Text, NumberStyles.Integer, provider, out _);
            var result32 = byte.TryParse(utf8Text, null, out _);
        }

        public void NoDetection()
        {
            
        }
    }
}