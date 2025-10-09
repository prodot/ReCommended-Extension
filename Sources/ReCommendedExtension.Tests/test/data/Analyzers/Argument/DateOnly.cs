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

            var result41 = DateOnly.Parse(s, style: DateTimeStyles.None, provider: null);

            var result51 = DateOnly.ParseExact(s, format, provider, style);
            var result52 = DateOnly.ParseExact(s, formats, provider, style);
            var result53 = DateOnly.ParseExact(s1, formats, provider, style);

            var result61 = DateOnly.TryParseExact(s, format, provider, style, out result);
            var result62 = DateOnly.TryParseExact(s1, format, provider, style, out result);
            var result63 = DateOnly.TryParseExact(s, formats, provider, style, out result);
            var result64 = DateOnly.TryParseExact(s1, formats, provider, style, out result);

            var result71 = DateOnly.ParseExact(s, ["d", "D", "m", "o", "r", "y"]);
            var result72 = DateOnly.ParseExact(s1, ["d", "D", "m", "o", "r", "y"]);
            var result73 = DateOnly.ParseExact(s, ["d", "D", "m", "o", "r", "y"], provider, style);
            var result74 = DateOnly.ParseExact(s1, ["d", "D", "m", "o", "r", "y"], provider, style);

            var result81 = DateOnly.TryParseExact(s, ["d", "D", "m", "o", "r", "y"], out result);
            var result82 = DateOnly.TryParseExact(s1, ["d", "D", "m", "o", "r", "y"], out result);
            var result83 = DateOnly.TryParseExact(s, ["d", "D", "m", "o", "r", "y"], provider, style, out result);
            var result84 = DateOnly.TryParseExact(s1, ["d", "D", "m", "o", "r", "y"], provider, style, out result);
        }
    }
}