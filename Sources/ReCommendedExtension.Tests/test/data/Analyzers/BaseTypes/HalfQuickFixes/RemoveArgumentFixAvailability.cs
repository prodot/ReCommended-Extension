using System;
using System.Globalization;

namespace Test
{
    public class Halves
    {
        public void Parse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = Half.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands);
            var result12 = Half.Parse(s, null);
            var result13 = Half.Parse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingWhite | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent | NumberStyles.AllowThousands, provider);
            var result14 = Half.Parse(s, style, null);
            var result15 = Half.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, null);

            var result21 = Half.Parse(s1, null);

            var result31 = Half.Parse(utf8Text, null);
        }

        public void ToString(byte number, string format, IFormatProvider provider)
        {
            var result1 = number.ToString(null as string);
            var result2 = number.ToString("");
            var result3 = number.ToString(null as IFormatProvider);
            var result4 = number.ToString(null, provider);
            var result5 = number.ToString("", provider);
            var result6 = number.ToString(format, null);
            var result7 = number.ToString("", null);
        }

        public void TryParse(string s, ReadOnlySpan<char> s1, ReadOnlySpan<byte> utf8Text, NumberStyles style, IFormatProvider provider)
        {
            var result11 = Half.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, provider, out _);
            var result12 = Half.TryParse(s, null, out _);

            var result21 = Half.TryParse(s1, NumberStyles.Float | NumberStyles.AllowThousands, provider, out _);
            var result22 = Half.TryParse(s1, null, out _);

            var result31 = Half.TryParse(utf8Text, NumberStyles.Float | NumberStyles.AllowThousands, provider, out _);
            var result32 = Half.TryParse(utf8Text, null, out _);
        }
    }
}