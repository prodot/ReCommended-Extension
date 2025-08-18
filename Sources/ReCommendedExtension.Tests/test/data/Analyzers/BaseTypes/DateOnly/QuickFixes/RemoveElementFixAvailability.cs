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
    }
}