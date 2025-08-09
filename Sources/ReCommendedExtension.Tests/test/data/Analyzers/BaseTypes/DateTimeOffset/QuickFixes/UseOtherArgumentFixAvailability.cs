using System;
using System.Globalization;

namespace Test
{
    public class DateTimeOffsets
    {
        public void ParseExact(string input, ReadOnlySpan<char> input1, string format, IFormatProvider formatProvider, DateTimeStyles styles)
        {
            var result11 = DateTimeOffset.ParseExact(input, "o", formatProvider);
            var result12 = DateTimeOffset.ParseExact(input, "O", formatProvider);
            var result13 = DateTimeOffset.ParseExact(input, "r", formatProvider);
            var result14 = DateTimeOffset.ParseExact(input, "R", formatProvider);
            var result15 = DateTimeOffset.ParseExact(input, "s", formatProvider);
            var result16 = DateTimeOffset.ParseExact(input, "u", formatProvider);

            var result21 = DateTimeOffset.ParseExact(input, "o", formatProvider, styles);
            var result22 = DateTimeOffset.ParseExact(input, "O", formatProvider, styles);
            var result23 = DateTimeOffset.ParseExact(input, "r", formatProvider, styles);
            var result24 = DateTimeOffset.ParseExact(input, "R", formatProvider, styles);
            var result25 = DateTimeOffset.ParseExact(input, "s", formatProvider, styles);
            var result26 = DateTimeOffset.ParseExact(input, "u", formatProvider, styles);

            var result31 = DateTimeOffset.ParseExact(input, [format], formatProvider, styles);
            var result32 = DateTimeOffset.ParseExact(input, (string[])[format], formatProvider, styles);
            var result33 = DateTimeOffset.ParseExact(input, new[] { format }, formatProvider, styles);
            var result34 = DateTimeOffset.ParseExact(input, new string[] { format }, formatProvider, styles);

            var result41 = DateTimeOffset.ParseExact(input, ["o", "r", "s", "u"], formatProvider, styles);
            var result42 = DateTimeOffset.ParseExact(input, (string[])["o", "r", "s", "u"], formatProvider, styles);
            var result43 = DateTimeOffset.ParseExact(input, new[] { "o", "r", "s", "u" }, formatProvider, styles);
            var result44 = DateTimeOffset.ParseExact(input, new string[] { "o", "r", "s", "u" }, formatProvider, styles);

            var result51 = DateTimeOffset.ParseExact(input1, ["o", "r", "s", "u"], formatProvider, styles);
            var result52 = DateTimeOffset.ParseExact(input1, (string[])["o", "r", "s", "u"], formatProvider, styles);
            var result53 = DateTimeOffset.ParseExact(input1, new[] { "o", "r", "s", "u" }, formatProvider, styles);
            var result54 = DateTimeOffset.ParseExact(input1, new string[] { "o", "r", "s", "u" }, formatProvider, styles);
        }

        public void TryParseExact(string input, ReadOnlySpan<char> input1, string format, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
        {
            var result11 = DateTimeOffset.TryParseExact(input, "o", formatProvider, styles, out result);
            var result12 = DateTimeOffset.TryParseExact(input, "O", formatProvider, styles, out result);
            var result13 = DateTimeOffset.TryParseExact(input, "r", formatProvider, styles, out result);
            var result14 = DateTimeOffset.TryParseExact(input, "R", formatProvider, styles, out result);
            var result15 = DateTimeOffset.TryParseExact(input, "s", formatProvider, styles, out result);
            var result16 = DateTimeOffset.TryParseExact(input, "u", formatProvider, styles, out result);

            var result21 = DateTimeOffset.TryParseExact(input, [format], formatProvider, styles, out result);
            var result22 = DateTimeOffset.TryParseExact(input, (string[])[format], formatProvider, styles, out result);
            var result23 = DateTimeOffset.TryParseExact(input, new[] { format }, formatProvider, styles, out result);
            var result24 = DateTimeOffset.TryParseExact(input, new string[] { format }, formatProvider, styles, out result);

            var result31 = DateTimeOffset.TryParseExact(input, ["o", "r", "s", "u"], formatProvider, styles, out result);
            var result32 = DateTimeOffset.TryParseExact(input, (string[])["o", "r", "s", "u"], formatProvider, styles, out result);
            var result33 = DateTimeOffset.TryParseExact(input, new[] { "o", "r", "s", "u" }, formatProvider, styles, out result);
            var result34 = DateTimeOffset.TryParseExact(input, new string[] { "o", "r", "s", "u" }, formatProvider, styles, out result);

            var result41 = DateTimeOffset.TryParseExact(input1, ["o", "r", "s", "u"], formatProvider, styles, out result);
            var result42 = DateTimeOffset.TryParseExact(input1, (string[])["o", "r", "s", "u"], formatProvider, styles, out result);
            var result43 = DateTimeOffset.TryParseExact(input1, new[] { "o", "r", "s", "u" }, formatProvider, styles, out result);
            var result44 = DateTimeOffset.TryParseExact(input1, new string[] { "o", "r", "s", "u" }, formatProvider, styles, out result);
        }
    }
}