using System;
using System.Globalization;

namespace Test
{
    public class Doubles
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = double.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands);
            var result12 = double.Parse(s, null);
            var result13 = double.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowThousands, provider);
            var result14 = double.Parse(s, style, null);
            var result15 = double.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, null);

            var result21 = double.Parse(s1, null);

            var result31 = double.Parse(utf8Text, null);
        }
    }
}