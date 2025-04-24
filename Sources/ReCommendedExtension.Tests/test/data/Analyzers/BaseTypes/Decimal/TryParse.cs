using System;
using System.Globalization;

namespace Test
{
    public class Decimals
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = decimal.TryParse(s, NumberStyles.Number, provider, out _);
            var result12 = decimal.TryParse(s, null, out _);

            var result21 = decimal.TryParse(s1, NumberStyles.Number, provider, out _);
            var result22 = decimal.TryParse(s1, null, out _);

            var result31 = decimal.TryParse(utf8Text, NumberStyles.Number, provider, out _);
            var result32 = decimal.TryParse(utf8Text, null, out _);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = decimal.TryParse(s, style, provider, out _);

            var result21 = decimal.TryParse(s1, style, provider, out _);

            var result31 = decimal.TryParse(utf8Text, style, provider, out _);
        }
    }
}