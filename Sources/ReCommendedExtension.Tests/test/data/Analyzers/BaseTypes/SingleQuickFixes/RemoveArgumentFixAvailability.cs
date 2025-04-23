using System;
using System.Globalization;

namespace Test
{
    public class Singles
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = float.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands);
            var result12 = float.Parse(s, null);
            var result13 = float.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowThousands, provider);
            var result14 = float.Parse(s, style, null);
            var result15 = float.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, null);

            var result21 = float.Parse(s1, null);

            var result31 = float.Parse(utf8Text, null);
        }
    }
}