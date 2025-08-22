using System;
using System.Globalization;

namespace Test
{
    public class DatesOnly
    {
        public void RedundantArgumentRange(string s, ReadOnlySpan<char> s1, string format, string[] formats, out DateOnly result)
        {
            var result11 = DateOnly.TryParseExact(s, format, null, DateTimeStyles.None, out result);
            var result12 = DateOnly.TryParseExact(s1, format, null, DateTimeStyles.None, out result);

            var result21 = DateOnly.TryParseExact(s, formats, null, DateTimeStyles.None, out result);
            var result22 = DateOnly.TryParseExact(s1, formats, null, DateTimeStyles.None, out result);
        }

        public void OtherArgument(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out DateOnly result)
        {
            var result1 = DateOnly.TryParseExact(s, "R", provider, style, out result);
            var result2 = DateOnly.TryParseExact(s, [format], out result);
            var result3 = DateOnly.TryParseExact(s, [format], provider, style, out result);
            var result4 = DateOnly.TryParseExact(s, ["o", "r"], provider, style, out result);
            var result5 = DateOnly.TryParseExact(s1, ["o", "r"], provider, style, out result);
        }

        public void RedundantElement(string s, ReadOnlySpan<char> s1, IFormatProvider provider, DateTimeStyles style, out DateOnly result)
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

        public void NoDetection(string s, ReadOnlySpan<char> s1, string format, IFormatProvider provider, DateTimeStyles style, out DateOnly result)
        {
            var result11 = DateOnly.TryParseExact(s, format, provider, style, out result);
            var result12 = DateOnly.TryParseExact(s1, format, provider, style, out result);

            var result21 = DateOnly.TryParseExact(s, [format, format], out result);
            var result22 = DateOnly.TryParseExact(s, ["d", "D", "m", "o", "r", "y"], out result);

            var result31 = DateOnly.TryParseExact(s1, [format, format], out result);
            var result32 = DateOnly.TryParseExact(s1, ["d", "D", "m", "o", "r", "y"], out result);

            var result41 = DateOnly.TryParseExact(s, [format, format], provider, style, out result);
            var result42 = DateOnly.TryParseExact(s1, [format, format], provider, style, out result);
        }
    }
}