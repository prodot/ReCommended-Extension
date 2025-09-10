using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void ParseExact(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = TimeOnly.ParseExact(s, ["t", "t", "T", "o", "O", "r", "R"]);
            var result12 = TimeOnly.ParseExact(s, (string[])["t", "t", "T", "o", "O", "r", "R"]);
            var result13 = TimeOnly.ParseExact(s, new[] { "t", "t", "T", "o", "O", "r", "R" });
            var result14 = TimeOnly.ParseExact(s, new string[] {"t", "t", "T", "o", "O", "r", "R" });

            var result21 = TimeOnly.ParseExact(s, ["t", "t", "T", "o", "O", "r", "R"], provider, style);
            var result22 = TimeOnly.ParseExact(s, (string[])["t", "t", "T", "o", "O", "r", "R"], provider, style);
            var result23 = TimeOnly.ParseExact(s, new[] {"t", "t", "T", "o", "O", "r", "R" }, provider, style);
            var result24 = TimeOnly.ParseExact(s, new string[] {"t", "t", "T", "o", "O", "r", "R" }, provider, style);

            var result31 = TimeOnly.ParseExact(s1, ["t", "t", "T", "o", "O", "r", "R"]);
            var result32 = TimeOnly.ParseExact(s1, (string[])["t", "t", "T", "o", "O", "r", "R"]);
            var result33 = TimeOnly.ParseExact(s1, new[] {"t", "t", "T", "o", "O", "r", "R" });
            var result34 = TimeOnly.ParseExact(s1, new string[] { "t", "t", "T", "o", "O", "r", "R" });

            var result41 = TimeOnly.ParseExact(s1, ["t", "t", "T", "o", "O", "r", "R"], provider, style);
            var result42 = TimeOnly.ParseExact(s1, (string[])["t", "t", "T", "o", "O", "r", "R"], provider, style);
            var result43 = TimeOnly.ParseExact(s1, new[] {"t", "t", "T", "o", "O", "r", "R" }, provider, style);
            var result44 = TimeOnly.ParseExact(s1, new string[] {"t", "t", "T", "o", "O", "r", "R" }, provider, style);
        }

        public void TryParseExact(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out TimeOnly result)
        {
            var result11 = TimeOnly.TryParseExact(s, ["t", "t", "T", "o", "O", "r", "R"], out result);
            var result12 = TimeOnly.TryParseExact(s, (string[])["t", "t", "T", "o", "O", "r", "R"], out result);
            var result13 = TimeOnly.TryParseExact(s, new[] { "t", "t", "T", "o", "O", "r", "R" }, out result);
            var result14 = TimeOnly.TryParseExact(s, new string[] { "t", "t", "T", "o", "O", "r", "R" }, out result);

            var result21 = TimeOnly.TryParseExact(s1, ["t", "t", "T", "o", "O", "r", "R"], out result);
            var result22 = TimeOnly.TryParseExact(s1, (string[])["t", "t", "T", "o", "O", "r", "R"], out result);
            var result23 = TimeOnly.TryParseExact(s1, new[] { "t", "t", "T", "o", "O", "r", "R" }, out result);
            var result24 = TimeOnly.TryParseExact(s1, new string[] { "t", "t", "T", "o", "O", "r", "R" }, out result);

            var result31 = TimeOnly.TryParseExact(s, ["t", "t", "T", "o", "O", "r", "R"], provider, style, out result);
            var result32 = TimeOnly.TryParseExact(s, (string[])["t", "t", "T", "o", "O", "r", "R"], provider, style, out result);
            var result33 = TimeOnly.TryParseExact(s, new[] { "t", "t", "T", "o", "O", "r", "R" }, provider, style, out result);
            var result34 = TimeOnly.TryParseExact(s, new string[] { "t", "t", "T", "o", "O", "r", "R" }, provider, style, out result);

            var result41 = TimeOnly.TryParseExact(s1, ["t", "t", "T", "o", "O", "r", "R"], provider, style, out result);
            var result42 = TimeOnly.TryParseExact(s1, (string[])["t", "t", "T", "o", "O", "r", "R"], provider, style, out result);
            var result43 = TimeOnly.TryParseExact(s1, new[] { "t", "t", "T", "o", "O", "r", "R" }, provider, style, out result);
            var result44 = TimeOnly.TryParseExact(s1, new string[] { "t", "t", "T", "o", "O", "r", "R" }, provider, style, out result);
        }
    }
}