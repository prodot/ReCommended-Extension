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

        public void Round(float x, int digits, MidpointRounding mode)
        {
            var result11 = float.Round(x, 0);
            var result12 = float.Round(x, 0, mode);
            var result13 = float.Round(x, MidpointRounding.ToEven);
            var result14 = float.Round(x, digits, MidpointRounding.ToEven);
            var result15 = float.Round(x, 0, MidpointRounding.ToEven);

            var result21 = MathF.Round(x, 0);
            var result22 = MathF.Round(x, 0, mode);
            var result23 = MathF.Round(x, MidpointRounding.ToEven);
            var result24 = MathF.Round(x, digits, MidpointRounding.ToEven);
            var result25 = MathF.Round(x, 0, MidpointRounding.ToEven);
        }

        public void TryParse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = float.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out _);
            var result12 = float.TryParse(s, null, out _);

            var result21 = float.TryParse(s1, NumberStyles.Float | NumberStyles.AllowThousands, provider, out _);
            var result22 = float.TryParse(s1, null, out _);

            var result31 = float.TryParse(utf8Text, NumberStyles.Float | NumberStyles.AllowThousands, provider, out _);
            var result32 = float.TryParse(utf8Text, null, out _);
        }
    }
}