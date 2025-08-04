using System;
using System.Globalization;

namespace Test
{
    public class DateTimeOffsets
    {
        public void RedundantArgument(string input, string format, IFormatProvider formatProvider)
        {
            var result = DateTimeOffset.ParseExact(input, format, formatProvider, DateTimeStyles.None);
        }

        public void OtherArgument(string input, ReadOnlySpan<char> input1, string format, IFormatProvider formatProvider, DateTimeStyles styles)
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

        public void RedundantElement(string input, ReadOnlySpan<char> input1, IFormatProvider formatProvider, DateTimeStyles styles)
        {
            var result11 = DateTimeOffset.ParseExact(input, ["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], formatProvider, styles);
            var result12 = DateTimeOffset.ParseExact(input, (string[])["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], formatProvider, styles);
            var result13 = DateTimeOffset.ParseExact(input, new[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" }, formatProvider, styles);
            var result14 = DateTimeOffset.ParseExact(input, new string[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" }, formatProvider, styles);

            var result21 = DateTimeOffset.ParseExact(input1, ["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], formatProvider, styles);
            var result22 = DateTimeOffset.ParseExact(input1, (string[])["d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y"], formatProvider, styles);
            var result23 = DateTimeOffset.ParseExact(input1, new[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" }, formatProvider, styles);
            var result24 = DateTimeOffset.ParseExact(input1, new string[] { "d", "D", "f", "F", "g", "g", "G", "m", "M", "o", "O", "r", "R", "s", "t", "T", "u", "y", "Y" }, formatProvider, styles);
        }

        public void NoDetection(string input, ReadOnlySpan<char> input1, string format, IFormatProvider formatProvider, DateTimeStyles styles)
        {
            var result11 = DateTimeOffset.ParseExact(input, format, formatProvider, styles);

            var result21 = DateTimeOffset.ParseExact(input, "o", null);
            var result22 = DateTimeOffset.ParseExact(input, "O", null);
            var result23 = DateTimeOffset.ParseExact(input, "r", null);
            var result24 = DateTimeOffset.ParseExact(input, "R", null);
            var result25 = DateTimeOffset.ParseExact(input, "s", null);
            var result26 = DateTimeOffset.ParseExact(input, "u", null);

            var result31 = DateTimeOffset.ParseExact(input, "o", null, styles);
            var result32 = DateTimeOffset.ParseExact(input, "O", null, styles);
            var result33 = DateTimeOffset.ParseExact(input, "r", null, styles);
            var result34 = DateTimeOffset.ParseExact(input, "R", null, styles);
            var result35 = DateTimeOffset.ParseExact(input, "s", null, styles);
            var result36 = DateTimeOffset.ParseExact(input, "u", null, styles);

            var result41 = DateTimeOffset.ParseExact(input, [format, format], formatProvider, styles);
            var result42 = DateTimeOffset.ParseExact(input, (string[])[format, format], formatProvider, styles);
            var result43 = DateTimeOffset.ParseExact(input, new[] { format, format }, formatProvider, styles);
            var result44 = DateTimeOffset.ParseExact(input, new string[] { format, format }, formatProvider, styles);

            var result51 = DateTimeOffset.ParseExact(input, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], formatProvider, styles);
            var result52 = DateTimeOffset.ParseExact(input, (string[])["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], formatProvider, styles);
            var result53 = DateTimeOffset.ParseExact(input, new[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y" }, formatProvider, styles);
            var result54 = DateTimeOffset.ParseExact(input, new string[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y" }, formatProvider, styles);

            var result61 = DateTimeOffset.ParseExact(input, ["g", "o", "r", "s", "u"], formatProvider, styles);
            var result62 = DateTimeOffset.ParseExact(input, (string[])["g", "o", "r", "s", "u"], formatProvider, styles);
            var result63 = DateTimeOffset.ParseExact(input, new[] { "g", "o", "r", "s", "u" }, formatProvider, styles);
            var result64 = DateTimeOffset.ParseExact(input, new string[] { "g", "o", "r", "s", "u" }, formatProvider, styles);

            var result71 = DateTimeOffset.ParseExact(input1, ["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], formatProvider, styles);
            var result72 = DateTimeOffset.ParseExact(input1, (string[])["d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y"], formatProvider, styles);
            var result73 = DateTimeOffset.ParseExact(input1, new[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y" }, formatProvider, styles);
            var result74 = DateTimeOffset.ParseExact(input1, new string[] { "d", "D", "f", "F", "g", "G", "m", "o", "r", "s", "t", "T", "u", "y" }, formatProvider, styles);

            var result81 = DateTimeOffset.ParseExact(input1, ["g", "o", "r", "s", "u"], formatProvider, styles);
            var result82 = DateTimeOffset.ParseExact(input1, (string[])["g", "o", "r", "s", "u"], formatProvider, styles);
            var result83 = DateTimeOffset.ParseExact(input1, new[] { "g", "o", "r", "s", "u" }, formatProvider, styles);
            var result84 = DateTimeOffset.ParseExact(input1, new string[] { "g", "o", "r", "s", "u" }, formatProvider, styles);
        }
    }
}