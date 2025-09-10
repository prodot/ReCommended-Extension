using System;
using System.Globalization;

namespace Test
{
    public class TimeSpans
    {
        public void OtherArgument(string input, string format, IFormatProvider formatProvider, TimeSpanStyles styles)
        {
            var result11 = TimeSpan.ParseExact(input, "c", formatProvider);
            var result12 = TimeSpan.ParseExact(input, "t", formatProvider);
            var result13 = TimeSpan.ParseExact(input, "T", formatProvider);

            var result21 = TimeSpan.ParseExact(input, "c", formatProvider, styles);
            var result22 = TimeSpan.ParseExact(input, "t", formatProvider, styles);
            var result23 = TimeSpan.ParseExact(input, "T", formatProvider, styles);

            var result31 = TimeSpan.ParseExact(input, [format], formatProvider);
            var result32 = TimeSpan.ParseExact(input, (string[])[format], formatProvider);
            var result33 = TimeSpan.ParseExact(input, new[] { format }, formatProvider);
            var result34 = TimeSpan.ParseExact(input, new string[] { format }, formatProvider);

            var result41 = TimeSpan.ParseExact(input, [format], formatProvider, styles);
            var result42 = TimeSpan.ParseExact(input, (string[])[format], formatProvider, styles);
            var result43 = TimeSpan.ParseExact(input, new[] { format }, formatProvider, styles);
            var result44 = TimeSpan.ParseExact(input, new string[] { format }, formatProvider, styles);
        }

        public void RedundantArgument(string input, string format, string[] formats, IFormatProvider formatProvider)
        {
            var result1 = TimeSpan.ParseExact(input, format, formatProvider, TimeSpanStyles.None);
            var result2 = TimeSpan.ParseExact(input, formats, formatProvider, TimeSpanStyles.None);
        }

        public void RedundantElement(string input, ReadOnlySpan<char> input2, IFormatProvider formatProvider, TimeSpanStyles styles)
        {
            var result11 = TimeSpan.ParseExact(input, ["c", "t", "T", "g", "g"], formatProvider);
            var result12 = TimeSpan.ParseExact(input, (string[])["c", "t", "T", "g", "g"], formatProvider);
            var result13 = TimeSpan.ParseExact(input, new[] { "c", "t", "T", "g", "g" }, formatProvider);
            var result14 = TimeSpan.ParseExact(input, new string[] { "c", "t", "T", "g", "g" }, formatProvider);

            var result21 = TimeSpan.ParseExact(input, ["c", "t", "T", "g", "g"], formatProvider, styles);
            var result22 = TimeSpan.ParseExact(input, (string[])["c", "t", "T", "g", "g"], formatProvider, styles);
            var result23 = TimeSpan.ParseExact(input, new[] { "c", "t", "T", "g", "g" }, formatProvider, styles);
            var result24 = TimeSpan.ParseExact(input, new string[] { "c", "t", "T", "g", "g" }, formatProvider, styles);

            var result31 = TimeSpan.ParseExact(input2, ["c", "t", "T", "g", "g"], formatProvider, styles);
            var result32 = TimeSpan.ParseExact(input2, (string[])["c", "t", "T", "g", "g"], formatProvider, styles);
            var result33 = TimeSpan.ParseExact(input2, new[] { "c", "t", "T", "g", "g" }, formatProvider, styles);
            var result34 = TimeSpan.ParseExact(input2, new string[] { "c", "t", "T", "g", "g" }, formatProvider, styles);
        }

        public void NoDetection(string input, ReadOnlySpan<char> input2, string format, string[] formats, IFormatProvider formatProvider, TimeSpanStyles styles)
        {
            var result11 = TimeSpan.ParseExact(input, "c", null);
            var result12 = TimeSpan.ParseExact(input, "t", null);
            var result13 = TimeSpan.ParseExact(input, "T", null);

            var result21 = TimeSpan.ParseExact(input, format, formatProvider, styles);
            var result22 = TimeSpan.ParseExact(input, format, formatProvider, TimeSpanStyles.AssumeNegative);

            var result31 = TimeSpan.ParseExact(input, formats, formatProvider, styles);
            var result32 = TimeSpan.ParseExact(input, formats, formatProvider, TimeSpanStyles.AssumeNegative);

            var result41 = TimeSpan.ParseExact(input, "c", null, styles);
            var result42 = TimeSpan.ParseExact(input, "t", null, styles);
            var result43 = TimeSpan.ParseExact(input, "T", null, styles);

            var result51 = TimeSpan.ParseExact(input, "c", null, TimeSpanStyles.AssumeNegative);
            var result52 = TimeSpan.ParseExact(input, "t", null, TimeSpanStyles.AssumeNegative);
            var result53 = TimeSpan.ParseExact(input, "T", null, TimeSpanStyles.AssumeNegative);

            var result61 = TimeSpan.ParseExact(input, [format, format], formatProvider);
            var result62 = TimeSpan.ParseExact(input, (string[])[format, format], formatProvider);
            var result63 = TimeSpan.ParseExact(input, new[] { format, format }, formatProvider);
            var result64 = TimeSpan.ParseExact(input, new string[] { format, format }, formatProvider);

            var result71 = TimeSpan.ParseExact(input, [format, format], formatProvider, styles);
            var result72 = TimeSpan.ParseExact(input, (string[])[format, format], formatProvider, styles);
            var result73 = TimeSpan.ParseExact(input, new[] { format, format }, formatProvider, styles);
            var result74 = TimeSpan.ParseExact(input, new string[] { format, format }, formatProvider, styles);

            var result81 = TimeSpan.ParseExact(input, ["c", "g", "G"], formatProvider);
            var result82 = TimeSpan.ParseExact(input, (string[])["c", "g", "G"], formatProvider);
            var result83 = TimeSpan.ParseExact(input, new[] { "c", "g", "G" }, formatProvider);
            var result84 = TimeSpan.ParseExact(input, new string[] { "c", "g", "G" }, formatProvider);

            var result91 = TimeSpan.ParseExact(input, ["c", "g", "G"], formatProvider, styles);
            var result92 = TimeSpan.ParseExact(input, (string[])["c", "g", "G"], formatProvider, styles);
            var result93 = TimeSpan.ParseExact(input, new[] { "c", "g", "G" }, formatProvider, styles);
            var result94 = TimeSpan.ParseExact(input, new string[] { "c", "g", "G" }, formatProvider, styles);

            var resultA1 = TimeSpan.ParseExact(input2, ["c", "g", "G"], formatProvider, styles);
            var resultA2 = TimeSpan.ParseExact(input2, (string[])["c", "g", "G"], formatProvider, styles);
            var resultA3 = TimeSpan.ParseExact(input2, new[] { "c", "g", "G" }, formatProvider, styles);
            var resultA4 = TimeSpan.ParseExact(input2, new string[] { "c", "g", "G" }, formatProvider, styles);
        }
    }
}