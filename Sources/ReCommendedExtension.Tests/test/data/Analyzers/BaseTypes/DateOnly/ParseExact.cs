using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void RedundantArgumentRange(string s, ReadOnlySpan<char> s1, string format, string[] formats)
        {
            var result1 = DateOnly.ParseExact(s, format, null, DateTimeStyles.None);
            var result2 = DateOnly.ParseExact(s, formats, null, DateTimeStyles.None);
            var result3 = DateOnly.ParseExact(s1, formats, null, DateTimeStyles.None);
        }

        public void RedundantArgument(string s, ReadOnlySpan<char> s1, string format, string[] formats)
        {
            var result1 = DateOnly.ParseExact(s, format, null);
            var result2 = DateOnly.ParseExact(s, formats, null);
            var result3 = DateOnly.ParseExact(s1, formats, null);
        }

        public void OtherArgument(string s, string format, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = DateOnly.ParseExact(s, "R", provider, style);
            var result12 = DateOnly.ParseExact(s, "R", provider);

            var result21 = DateOnly.ParseExact(s, [format]);

            var result31 = DateOnly.ParseExact(s, [format], provider, style);
            var result32 = DateOnly.ParseExact(s, ["o", "r"], provider, style);
        }

        public void RedundantElement(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style)
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

        public void NoDetection(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style)
        {
            var result11 = DateOnly.ParseExact(s, format, provider, style);
            var result12 = DateOnly.ParseExact(s, format, provider);

            var result21 = DateOnly.ParseExact(s, [format, format]);
            var result22 = DateOnly.ParseExact(s, ["d", "D", "m", "o", "r", "y"]);

            var result31 = DateOnly.ParseExact(s, [format, format], provider, style);
            var result32 = DateOnly.ParseExact(s, ["d", "D", "m", "o", "r", "y"], provider, style);

            var result41 = DateOnly.ParseExact(s1, [format, format]);
            var result42 = DateOnly.ParseExact(s1, ["d", "D", "m", "o", "r", "y"]);

            var result51 = DateOnly.ParseExact(s1, [format, format], provider, style);
            var result52 = DateOnly.ParseExact(s1, ["d", "D", "m", "o", "r", "y"], provider, style);
        }
    }
}