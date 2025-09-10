using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void ParseExact(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = DateTime.ParseExact(s, "o", provider);
            var result12 = DateTime.ParseExact(s, "O", provider);
            var result13 = DateTime.ParseExact(s, "r", provider);
            var result14 = DateTime.ParseExact(s, "R", provider);
            var result15 = DateTime.ParseExact(s, "s", provider);
            var result16 = DateTime.ParseExact(s, "u", provider);

            var result21 = DateTime.ParseExact(s, "o", provider, style);
            var result22 = DateTime.ParseExact(s, "O", provider, style);
            var result23 = DateTime.ParseExact(s, "r", provider, style);
            var result24 = DateTime.ParseExact(s, "R", provider, style);
            var result25 = DateTime.ParseExact(s, "s", provider, style);
            var result26 = DateTime.ParseExact(s, "u", provider, style);

            var result31 = DateTime.ParseExact(s, [format], provider, style);
            var result32 = DateTime.ParseExact(s, (string[])[format], provider, style);
            var result33 = DateTime.ParseExact(s, new[] { format }, provider, style);
            var result34 = DateTime.ParseExact(s, new string[] { format }, provider, style);

            var result41 = DateTime.ParseExact(s, ["o", "r", "s", "u"], provider, style);
            var result42 = DateTime.ParseExact(s, (string[])["o", "r", "s", "u"], provider, style);
            var result43 = DateTime.ParseExact(s, new[] { "o", "r", "s", "u" }, provider, style);
            var result44 = DateTime.ParseExact(s, new string[] { "o", "r", "s", "u" }, provider, style);

            var result51 = DateTime.ParseExact(s1, ["o", "r", "s", "u"], provider, style);
            var result52 = DateTime.ParseExact(s1, (string[])["o", "r", "s", "u"], provider, style);
            var result53 = DateTime.ParseExact(s1, new[] { "o", "r", "s", "u" }, provider, style);
            var result54 = DateTime.ParseExact(s1, new string[] { "o", "r", "s", "u" }, provider, style);
        }

        public void TryParseExact(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result)
        {
            var result11 = DateTime.TryParseExact(s, "o", provider, style, out result);
            var result12 = DateTime.TryParseExact(s, "O", provider, style, out result);
            var result13 = DateTime.TryParseExact(s, "r", provider, style, out result);
            var result14 = DateTime.TryParseExact(s, "R", provider, style, out result);
            var result15 = DateTime.TryParseExact(s, "s", provider, style, out result);
            var result16 = DateTime.TryParseExact(s, "u", provider, style, out result);

            var result21 = DateTime.TryParseExact(s, [format], provider, style, out result);
            var result22 = DateTime.TryParseExact(s, (string[])[format], provider, style, out result);
            var result23 = DateTime.TryParseExact(s, new[] { format }, provider, style, out result);
            var result24 = DateTime.TryParseExact(s, new string[] { format }, provider, style, out result);

            var result31 = DateTime.TryParseExact(s, ["o", "r", "s", "u"], provider, style, out result);
            var result32 = DateTime.TryParseExact(s, (string[])["o", "r", "s", "u"], provider, style, out result);
            var result33 = DateTime.TryParseExact(s, new[] { "o", "r", "s", "u" }, provider, style, out result);
            var result34 = DateTime.TryParseExact(s, new string[] { "o", "r", "s", "u" }, provider, style, out result);

            var result41 = DateTime.TryParseExact(s1, ["o", "r", "s", "u"], provider, style, out result);
            var result42 = DateTime.TryParseExact(s1, (string[])["o", "r", "s", "u"], provider, style, out result);
            var result43 = DateTime.TryParseExact(s1, new[] { "o", "r", "s", "u" }, provider, style, out result);
            var result44 = DateTime.TryParseExact(s1, new string[] { "o", "r", "s", "u" }, provider, style, out result);
        }
    }
}