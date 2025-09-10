using System;
using System.Globalization;

namespace Test
{
    public class DateTimeOffsets
    {
        public void OtherArgument(string input, ReadOnlySpan<char> input1, string format, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
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

        public void RedundantElement(string input, ReadOnlySpan<char> input1, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
        {
            var result11 = DateTimeOffset.TryParseExact(input, ["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], formatProvider, styles, out result);
            var result12 = DateTimeOffset.TryParseExact(input, (string[])["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], formatProvider, styles, out result);
            var result13 = DateTimeOffset.TryParseExact(input, new[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" }, formatProvider, styles, out result);
            var result14 = DateTimeOffset.TryParseExact(input, new string[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" }, formatProvider, styles, out result);

            var result21 = DateTimeOffset.TryParseExact(input1, ["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], formatProvider, styles, out result);
            var result22 = DateTimeOffset.TryParseExact(input1, (string[])["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], formatProvider, styles, out result);
            var result23 = DateTimeOffset.TryParseExact(input1, new[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" }, formatProvider, styles, out result);
            var result24 = DateTimeOffset.TryParseExact(input1, new string[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" }, formatProvider, styles, out result);
        }

        public void NoDetection(string input, ReadOnlySpan<char> input1, string format, IFormatProvider formatProvider, DateTimeStyles styles, out DateTimeOffset result)
        {
            var result11 = DateTimeOffset.TryParseExact(input, format, formatProvider, styles, out result);

            var result21 = DateTimeOffset.TryParseExact(input, "o", null, styles, out result);
            var result22 = DateTimeOffset.TryParseExact(input, "O", null, styles, out result);
            var result23 = DateTimeOffset.TryParseExact(input, "r", null, styles, out result);
            var result24 = DateTimeOffset.TryParseExact(input, "R", null, styles, out result);
            var result25 = DateTimeOffset.TryParseExact(input, "s", null, styles, out result);
            var result26 = DateTimeOffset.TryParseExact(input, "u", null, styles, out result);

            var result31 = DateTimeOffset.TryParseExact(input, [format, format], formatProvider, styles, out result);
            var result32 = DateTimeOffset.TryParseExact(input, (string[])[format, format], formatProvider, styles, out result);
            var result33 = DateTimeOffset.TryParseExact(input, new[] { format, format }, formatProvider, styles, out result);
            var result34 = DateTimeOffset.TryParseExact(input, new string[] { format, format }, formatProvider, styles, out result);

            var result41 = DateTimeOffset.TryParseExact(input, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], formatProvider, styles, out result);
            var result42 = DateTimeOffset.TryParseExact(input, (string[])["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], formatProvider, styles, out result);
            var result43 = DateTimeOffset.TryParseExact(input, new[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y" }, formatProvider, styles, out result);
            var result44 = DateTimeOffset.TryParseExact(input, new string[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y" }, formatProvider, styles, out result);

            var result51 = DateTimeOffset.TryParseExact(input, ["g", "o", "r", "s", "u"], formatProvider, styles, out result);
            var result52 = DateTimeOffset.TryParseExact(input, (string[])["g", "o", "r", "s", "u"], formatProvider, styles, out result);
            var result53 = DateTimeOffset.TryParseExact(input, new[] { "g", "o", "r", "s", "u" }, formatProvider, styles, out result);
            var result54 = DateTimeOffset.TryParseExact(input, new string[] { "g", "o", "r", "s", "u" }, formatProvider, styles, out result);

            var result61 = DateTimeOffset.TryParseExact(input1, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], formatProvider, styles, out result);
            var result62 = DateTimeOffset.TryParseExact(input1, (string[])["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], formatProvider, styles, out result);
            var result63 = DateTimeOffset.TryParseExact(input1, new[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y" }, formatProvider, styles, out result);
            var result64 = DateTimeOffset.TryParseExact(input1, new string[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y" }, formatProvider, styles, out result);

            var result71 = DateTimeOffset.TryParseExact(input1, ["g", "o", "r", "s", "u"], formatProvider, styles, out result);
            var result72 = DateTimeOffset.TryParseExact(input1, (string[])["g", "o", "r", "s", "u"], formatProvider, styles, out result);
            var result73 = DateTimeOffset.TryParseExact(input1, new[] { "g", "o", "r", "s", "u" }, formatProvider, styles, out result);
            var result74 = DateTimeOffset.TryParseExact(input1, new string[] { "g", "o", "r", "s", "u" }, formatProvider, styles, out result);
        }
    }
}