using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void RedundantArgument(string s, string format, IFormatProvider provider)
        {
            var result = DateTime.ParseExact(s, format, provider, DateTimeStyles.None);
        }

        public void OtherArgument(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style)
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

        public void RedundantElement(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style)
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

        public void NoDetection(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = DateTime.ParseExact(s, format, provider, style);

            var result21 = DateTime.ParseExact(s, "o", null);
            var result22 = DateTime.ParseExact(s, "O", null);
            var result23 = DateTime.ParseExact(s, "r", null);
            var result24 = DateTime.ParseExact(s, "R", null);
            var result25 = DateTime.ParseExact(s, "s", null);
            var result26 = DateTime.ParseExact(s, "u", null);

            var result31 = DateTime.ParseExact(s, "o", null, style);
            var result32 = DateTime.ParseExact(s, "O", null, style);
            var result33 = DateTime.ParseExact(s, "r", null, style);
            var result34 = DateTime.ParseExact(s, "R", null, style);
            var result35 = DateTime.ParseExact(s, "s", null, style);
            var result36 = DateTime.ParseExact(s, "u", null, style);

            var result41 = DateTime.ParseExact(s, [format, format], provider, style);
            var result42 = DateTime.ParseExact(s, (string[])[format, format], provider, style);
            var result43 = DateTime.ParseExact(s, new[] { format, format }, provider, style);
            var result44 = DateTime.ParseExact(s, new string[] { format, format }, provider, style);

            var result51 = DateTime.ParseExact(s, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style);
            var result52 = DateTime.ParseExact(s, (string[])["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style);
            var result53 = DateTime.ParseExact(s, new[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y" }, provider, style);
            var result54 = DateTime.ParseExact(s, new string[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y" }, provider, style);

            var result61 = DateTime.ParseExact(s, ["g", "o", "r", "s", "u"], provider, style);
            var result62 = DateTime.ParseExact(s, (string[])["g", "o", "r", "s", "u"], provider, style);
            var result63 = DateTime.ParseExact(s, new[] { "g", "o", "r", "s", "u" }, provider, style);
            var result64 = DateTime.ParseExact(s, new string[] { "g", "o", "r", "s", "u" }, provider, style);

            var result71 = DateTime.ParseExact(s1, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style);
            var result72 = DateTime.ParseExact(s1, (string[])["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y"], provider, style);
            var result73 = DateTime.ParseExact(s1, new[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y" }, provider, style);
            var result74 = DateTime.ParseExact(s1, new string[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "U", "y" }, provider, style);

            var result81 = DateTime.ParseExact(s1, ["g", "o", "r", "s", "u"], provider, style);
            var result82 = DateTime.ParseExact(s1, (string[])["g", "o", "r", "s", "u"], provider, style);
            var result83 = DateTime.ParseExact(s1, new[] { "g", "o", "r", "s", "u" }, provider, style);
            var result84 = DateTime.ParseExact(s1, new string[] { "g", "o", "r", "s", "u" }, provider, style);
        }
    }
}