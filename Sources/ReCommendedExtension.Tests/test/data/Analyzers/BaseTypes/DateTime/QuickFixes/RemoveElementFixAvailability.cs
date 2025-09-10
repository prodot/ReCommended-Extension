using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void ParseExact(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = DateTime.ParseExact(s, ["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style);
            var result12 = DateTime.ParseExact(s, (string[])["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style);
            var result13 = DateTime.ParseExact(s, new[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" }, provider, style);
            var result14 = DateTime.ParseExact(s, new string[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" }, provider, style);

            var result21 = DateTime.ParseExact(s1, ["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style);
            var result22 = DateTime.ParseExact(s1, (string[])["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style);
            var result23 = DateTime.ParseExact(s1, new[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" }, provider, style);
            var result24 = DateTime.ParseExact(s1, new string[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" }, provider, style);
        }

        public void TryParseExact(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out DateTime result)
        {
            var result11 = DateTime.TryParseExact(s, ["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style, out result);
            var result12 = DateTime.TryParseExact(s, (string[])["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style, out result);
            var result13 = DateTime.TryParseExact(s, new[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" }, provider, style, out result);
            var result14 = DateTime.TryParseExact(s, new string[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" }, provider, style, out result);

            var result21 = DateTime.TryParseExact(s1, ["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style, out result);
            var result22 = DateTime.TryParseExact(s1, (string[])["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y"], provider, style, out result);
            var result23 = DateTime.TryParseExact(s1, new[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" }, provider, style, out result);
            var result24 = DateTime.TryParseExact(s1, new string[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "U", "y", "Y" }, provider, style, out result);
        }
    }
}