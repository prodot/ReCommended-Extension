using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void OtherArgument(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result)
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

        public void RedundantElement(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out DateTime result)
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

        public void NoDetection(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out DateTime result)
        {
            var result11 = DateTime.TryParseExact(s, format, provider, style, out result);

            var result21 = DateTime.TryParseExact(s, "o", null, style, out result);
            var result22 = DateTime.TryParseExact(s, "O", null, style, out result);
            var result23 = DateTime.TryParseExact(s, "r", null, style, out result);
            var result24 = DateTime.TryParseExact(s, "R", null, style, out result);
            var result25 = DateTime.TryParseExact(s, "s", null, style, out result);
            var result26 = DateTime.TryParseExact(s, "u", null, style, out result);

            var result31 = DateTime.TryParseExact(s, [format, format], provider, style, out result);
            var result32 = DateTime.TryParseExact(s, (string[])[format, format], provider, style, out result);
            var result33 = DateTime.TryParseExact(s, new[] { format, format }, provider, style, out result);
            var result34 = DateTime.TryParseExact(s, new string[] { format, format }, provider, style, out result);

            var result41 = DateTime.TryParseExact(s, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style, out result);
            var result42 = DateTime.TryParseExact(s, (string[])["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style, out result);
            var result43 = DateTime.TryParseExact(s, new[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y" }, provider, style, out result);
            var result44 = DateTime.TryParseExact(s, new string[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y" }, provider, style, out result);

            var result51 = DateTime.TryParseExact(s, ["g", "o", "r", "s", "u"], provider, style, out result);
            var result52 = DateTime.TryParseExact(s, (string[])["g", "o", "r", "s", "u"], provider, style, out result);
            var result53 = DateTime.TryParseExact(s, new[] { "g", "o", "r", "s", "u" }, provider, style, out result);
            var result54 = DateTime.TryParseExact(s, new string[] { "g", "o", "r", "s", "u" }, provider, style, out result);

            var result61 = DateTime.TryParseExact(s1, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style, out result);
            var result62 = DateTime.TryParseExact(s1, (string[])["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style, out result);
            var result63 = DateTime.TryParseExact(s1, new[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y" }, provider, style, out result);
            var result64 = DateTime.TryParseExact(s1, new string[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y" }, provider, style, out result);

            var result71 = DateTime.TryParseExact(s1, ["g", "o", "r", "s", "u"], provider, style, out result);
            var result72 = DateTime.TryParseExact(s1, (string[])["g", "o", "r", "s", "u"], provider, style, out result);
            var result73 = DateTime.TryParseExact(s1, new[] { "g", "o", "r", "s", "u" }, provider, style, out result);
            var result74 = DateTime.TryParseExact(s1, new string[] { "g", "o", "r", "s", "u" }, provider, style, out result);
        }
    }
}