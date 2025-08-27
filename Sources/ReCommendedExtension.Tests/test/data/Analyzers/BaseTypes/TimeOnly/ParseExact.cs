using System;
using System.Globalization;

namespace Test
{
    public class TimesOnly
    {
        public void RedundantArgumentRange(string s, ReadOnlySpan<char> s1, string format, string[] formats)
        {
            var result1 = TimeOnly.ParseExact(s, format, null, DateTimeStyles.None);
            var result2 = TimeOnly.ParseExact(s, formats, null, DateTimeStyles.None);
            var result3 = TimeOnly.ParseExact(s1, formats, null, DateTimeStyles.None);
        }

        public void RedundantArgument(string s, ReadOnlySpan<char> s1, string format, string[] formats)
        {
            var result1 = TimeOnly.ParseExact(s, format, null);
            var result2 = TimeOnly.ParseExact(s, formats, null);
            var result3 = TimeOnly.ParseExact(s1, formats, null);
        }

        public void OtherArgument(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = TimeOnly.ParseExact(s, "R", provider, style);
            var result12 = TimeOnly.ParseExact(s, "R", provider);

            var result21 = TimeOnly.ParseExact(s, [format]);

            var result31 = TimeOnly.ParseExact(s, [format], provider, style);
            var result32 = TimeOnly.ParseExact(s, ["o", "r"], provider, style);

            var result41 = TimeOnly.ParseExact(s1, ["o", "r"], provider, style);
        }

        public void RedundantElement(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style)
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
            var result34 = TimeOnly.ParseExact(s1, new string[] {"t", "t", "T", "o", "O", "r", "R" });

            var result41 = TimeOnly.ParseExact(s1, ["t", "t", "T", "o", "O", "r", "R"], provider, style);
            var result42 = TimeOnly.ParseExact(s1, (string[])["t", "t", "T", "o", "O", "r", "R"], provider, style);
            var result43 = TimeOnly.ParseExact(s1, new[] {"t", "t", "T", "o", "O", "r", "R" }, provider, style);
            var result44 = TimeOnly.ParseExact(s1, new string[] {"t", "t", "T", "o", "O", "r", "R" }, provider, style);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = TimeOnly.ParseExact(s, format, provider, style);
            var result12 = TimeOnly.ParseExact(s, format, provider);

            var result21 = TimeOnly.ParseExact(s, [format, format]);
            var result22 = TimeOnly.ParseExact(s, ["t", "T", "o", "r"]);

            var result31 = TimeOnly.ParseExact(s, [format, format], provider, style);
            var result32 = TimeOnly.ParseExact(s, ["t", "T", "o", "r"], provider, style);

            var result41 = TimeOnly.ParseExact(s1, [format, format]);
            var result42 = TimeOnly.ParseExact(s1, ["t", "T", "o", "r"]);

            var result51 = TimeOnly.ParseExact(s1, [format, format], provider, style);
            var result52 = TimeOnly.ParseExact(s1, ["t", "T", "o", "r"], provider, style);
        }
    }
}