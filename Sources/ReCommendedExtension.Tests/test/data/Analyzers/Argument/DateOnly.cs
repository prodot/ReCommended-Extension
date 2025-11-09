using System;
using System.Globalization;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, string format, string[] formats, IFormatProvider provider, out DateOnly result)
        {
            var result11 = DateOnly.Parse(s, null);
            var result12 = DateOnly.Parse(s1, null);

            var result21 = DateOnly.ParseExact(s, format, null);
            var result22 = DateOnly.ParseExact(s, formats, null);
            var result23 = DateOnly.ParseExact(s1, formats, null);

            var result31 = DateOnly.TryParse(s, null, out result);
            var result32 = DateOnly.TryParse(s1, null, out result);
            var result33 = DateOnly.TryParse(s, provider, DateTimeStyles.None, out result);
            var result34 = DateOnly.TryParse(s1, provider, DateTimeStyles.None, out result);
        }

        public void RedundantArgumentRange(string s, ReadOnlySpan<char> s1, string format, string[] formats, out DateOnly result)
        {
            var result11 = DateOnly.Parse(s, null, DateTimeStyles.None);

            var result21 = DateOnly.ParseExact(s, format, null, DateTimeStyles.None);
            var result22 = DateOnly.ParseExact(s, formats, null, DateTimeStyles.None);
            var result23 = DateOnly.ParseExact(s1, formats, null, DateTimeStyles.None);

            var result31 = DateOnly.TryParseExact(s, format, null, DateTimeStyles.None, out result);
            var result32 = DateOnly.TryParseExact(s1, format, null, DateTimeStyles.None, out result);
            var result33 = DateOnly.TryParseExact(s, formats, null, DateTimeStyles.None, out result);
            var result34 = DateOnly.TryParseExact(s1, formats, null, DateTimeStyles.None, out result);
        }

        public void RedundantCollectionElement(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out DateOnly result)
        {
            var result11 = DateOnly.ParseExact(s, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"]);
            var result12 = DateOnly.ParseExact(s1, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"]);
            var result13 = DateOnly.ParseExact(s, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style);
            var result14 = DateOnly.ParseExact(s1, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style);

            var result21 = DateOnly.TryParseExact(s, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], out result);
            var result22 = DateOnly.TryParseExact(s1, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], out result);
            var result23 = DateOnly.TryParseExact(s, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style, out result);
            var result24 = DateOnly.TryParseExact(s1, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style, out result);
        }

        public void OtherArgument(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out DateOnly result)
        {
            var result11 = DateOnly.ParseExact(s, [format]);
            var result12 = DateOnly.ParseExact(s, "o", provider, style);
            var result13 = DateOnly.ParseExact(s, "O", provider, style);
            var result14 = DateOnly.ParseExact(s, "r", provider, style);
            var result15 = DateOnly.ParseExact(s, "R", provider, style);
            var result16 = DateOnly.ParseExact(s, [format], provider, style);
            var result17 = DateOnly.ParseExact(s, ["o", "r"], provider, style);
            var result18 = DateOnly.ParseExact(s1, ["O", "R"], provider, style);

            var result21 = DateOnly.TryParseExact(s, [format], out result);
            var result22 = DateOnly.TryParseExact(s, "o", provider, style, out result);
            var result23 = DateOnly.TryParseExact(s, "O", provider, style, out result);
            var result24 = DateOnly.TryParseExact(s, "r", provider, style, out result);
            var result25 = DateOnly.TryParseExact(s, "R", provider, style, out result);
            var result26 = DateOnly.TryParseExact(s, [format], provider, style, out result);
            var result27 = DateOnly.TryParseExact(s, ["o", "r"], provider, style, out result);
            var result28 = DateOnly.TryParseExact(s1, ["O", "R"], provider, style, out result);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, string format, string[] formats, IFormatProvider provider, DateTimeStyles style, out DateOnly result)
        {
            var result11 = DateOnly.Parse(s, provider, style);
            var result12 = DateOnly.Parse(s, provider);
            var result13 = DateOnly.Parse(s1, provider);

            var result21 = DateOnly.ParseExact(s, format, provider, style);
            var result22 = DateOnly.ParseExact(s, formats, provider, style);
            var result23 = DateOnly.ParseExact(s1, formats, provider, style);

            var result31 = DateOnly.TryParse(s, provider, out result);
            var result32 = DateOnly.TryParse(s1, provider, out result);
            var result33 = DateOnly.TryParse(s, provider, style, out result);
            var result34 = DateOnly.TryParse(s1, provider, style, out result);

            var result41 = DateOnly.ParseExact(s, format, provider, style);
            var result42 = DateOnly.ParseExact(s, formats, provider, style);
            var result43 = DateOnly.ParseExact(s1, formats, provider, style);

            var result51 = DateOnly.TryParseExact(s, format, provider, style, out result);
            var result52 = DateOnly.TryParseExact(s1, format, provider, style, out result);
            var result53 = DateOnly.TryParseExact(s, formats, provider, style, out result);
            var result54 = DateOnly.TryParseExact(s1, formats, provider, style, out result);

            var result61 = DateOnly.ParseExact(s, ["d", "D", "m", "o", "r", "y"]);
            var result62 = DateOnly.ParseExact(s1, ["d", "D", "m", "o", "r", "y"]);
            var result63 = DateOnly.ParseExact(s, ["d", "D", "m", "o", "r", "y"], provider, style);
            var result64 = DateOnly.ParseExact(s1, ["d", "D", "m", "o", "r", "y"], provider, style);

            var result71 = DateOnly.TryParseExact(s, ["d", "D", "m", "o", "r", "y"], out result);
            var result72 = DateOnly.TryParseExact(s1, ["d", "D", "m", "o", "r", "y"], out result);
            var result73 = DateOnly.TryParseExact(s, ["d", "D", "m", "o", "r", "y"], provider, style, out result);
            var result74 = DateOnly.TryParseExact(s1, ["d", "D", "m", "o", "r", "y"], provider, style, out result);

            var result81 = DateOnly.ParseExact(s, [format, format]);
            var result82 = DateOnly.ParseExact(s, "o", null, style);
            var result83 = DateOnly.ParseExact(s, "O", null, style);
            var result84 = DateOnly.ParseExact(s, "r", null, style);
            var result85 = DateOnly.ParseExact(s, "R", null, style);
            var result86 = DateOnly.ParseExact(s, [format, format], provider, style);
            var result87 = DateOnly.ParseExact(s, ["o", "r"], null, style);
            var result88 = DateOnly.ParseExact(s, ["d", "o", "r"], provider, style);
            var result89 = DateOnly.ParseExact(s1, ["o", "r"], null, style);
            var result8A = DateOnly.ParseExact(s1, ["o", "r", "d"], provider, style);

            var result91 = DateOnly.TryParseExact(s, [format, format], out result);
            var result92 = DateOnly.TryParseExact(s, "o", null, style, out result);
            var result93 = DateOnly.TryParseExact(s, "O", null, style, out result);
            var result94 = DateOnly.TryParseExact(s, "r", null, style, out result);
            var result95 = DateOnly.TryParseExact(s, "R", null, style, out result);
            var result96 = DateOnly.TryParseExact(s, [format, format], provider, style, out result);
            var result97 = DateOnly.TryParseExact(s, ["o", "r"], null, style, out result);
            var result98 = DateOnly.TryParseExact(s, ["d", "o", "r"], provider, style, out result);
            var result99 = DateOnly.TryParseExact(s1, ["o", "r"], null, style, out result);
            var result9A = DateOnly.TryParseExact(s1, ["o", "r", "d"], provider, style, out result);
        }
    }
}