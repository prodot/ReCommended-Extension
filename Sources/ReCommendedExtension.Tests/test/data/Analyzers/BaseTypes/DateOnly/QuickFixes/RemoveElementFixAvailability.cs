using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void ParseExact(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = DateOnly.ParseExact(s, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"]);
            var result12 = DateOnly.ParseExact(s, (string[])["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"]);
            var result13 = DateOnly.ParseExact(s, new[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" });
            var result14 = DateOnly.ParseExact(s, new string[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" });

            var result21 = DateOnly.ParseExact(s, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style);
            var result22 = DateOnly.ParseExact(s, (string[])["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style);
            var result23 = DateOnly.ParseExact(s, new[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, provider, style);
            var result24 = DateOnly.ParseExact(s, new string[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, provider, style);

            var result31 = DateOnly.ParseExact(s1, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"]);
            var result32 = DateOnly.ParseExact(s1, (string[])["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"]);
            var result33 = DateOnly.ParseExact(s1, new[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" });
            var result34 = DateOnly.ParseExact(s1, new string[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" });

            var result41 = DateOnly.ParseExact(s1, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style);
            var result42 = DateOnly.ParseExact(s1, (string[])["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style);
            var result43 = DateOnly.ParseExact(s1, new[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, provider, style);
            var result44 = DateOnly.ParseExact(s1, new string[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, provider, style);
        }

        public void TryParseExact(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out DateOnly result)
        {
            var result11 = DateOnly.TryParseExact(s, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], out result);
            var result12 = DateOnly.TryParseExact(s, (string[])["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], out result);
            var result13 = DateOnly.TryParseExact(s, new[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, out result);
            var result14 = DateOnly.TryParseExact(s, new string[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, out result);

            var result21 = DateOnly.TryParseExact(s1, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], out result);
            var result22 = DateOnly.TryParseExact(s1, (string[])["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], out result);
            var result23 = DateOnly.TryParseExact(s1, new[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, out result);
            var result24 = DateOnly.TryParseExact(s1, new string[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, out result);

            var result31 = DateOnly.TryParseExact(s, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style, out result);
            var result32 = DateOnly.TryParseExact(s, (string[])["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style, out result);
            var result33 = DateOnly.TryParseExact(s, new[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, provider, style, out result);
            var result34 = DateOnly.TryParseExact(s, new string[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, provider, style, out result);

            var result41 = DateOnly.TryParseExact(s1, ["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style, out result);
            var result42 = DateOnly.TryParseExact(s1, (string[])["d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y"], provider, style, out result);
            var result43 = DateOnly.TryParseExact(s1, new[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, provider, style, out result);
            var result44 = DateOnly.TryParseExact(s1, new string[] { "d", "d", "D", "m", "M", "o", "O", "r", "R", "y", "Y" }, provider, style, out result);
        }
    }
}