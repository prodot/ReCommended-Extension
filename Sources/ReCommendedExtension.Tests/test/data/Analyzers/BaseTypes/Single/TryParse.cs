using System;
using System.Globalization;

namespace Test
{
    public class Singles
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out _);
            var result12 = float.TryParse(s, null, out _);

            var result21 = float.TryParse(s1, NumberStyles.Float | NumberStyles.AllowThousands, provider, out _);
            var result22 = float.TryParse(s1, null, out _);

            var result31 = float.TryParse(utf8Text, NumberStyles.Float | NumberStyles.AllowThousands, provider, out _);
            var result32 = float.TryParse(utf8Text, null, out _);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = float.TryParse(s, style, provider, out _);

            var result21 = float.TryParse(s1, style, provider, out _);

            var result31 = float.TryParse(utf8Text, style, provider, out _);
        }
    }
}