using System;
using System.Globalization;

namespace Test
{
    public class TimeSpans
    {
        public void OtherArgument(string input, string format, IFormatProvider formatProvider, TimeSpanStyles styles, out TimeSpan result)
        {
            var result11 = TimeSpan.TryParseExact(input, "c", formatProvider, out result);
            var result12 = TimeSpan.TryParseExact(input, "t", formatProvider, out result);
            var result13 = TimeSpan.TryParseExact(input, "T", formatProvider, out result);

            var result21 = TimeSpan.TryParseExact(input, "c", formatProvider, styles, out result);
            var result22 = TimeSpan.TryParseExact(input, "t", formatProvider, styles, out result);
            var result23 = TimeSpan.TryParseExact(input, "T", formatProvider, styles, out result);

            var result31 = TimeSpan.TryParseExact(input, [format], formatProvider, out result);
            var result32 = TimeSpan.TryParseExact(input, (string[])[format], formatProvider, out result);
            var result33 = TimeSpan.TryParseExact(input, new[] { format }, formatProvider, out result);
            var result34 = TimeSpan.TryParseExact(input, new string[] { format }, formatProvider, out result);

            var result41 = TimeSpan.TryParseExact(input, [format], formatProvider, styles, out result);
            var result42 = TimeSpan.TryParseExact(input, (string[])[format], formatProvider, styles, out result);
            var result43 = TimeSpan.TryParseExact(input, new[] { format }, formatProvider, styles, out result);
            var result44 = TimeSpan.TryParseExact(input, new string[] { format }, formatProvider, styles, out result);
        }

        public void RedundantArgument(string input, ReadOnlySpan<char> input2, string format, string[] formats, IFormatProvider formatProvider, out TimeSpan result)
        {
            var result1 = TimeSpan.TryParseExact(input, format, formatProvider, TimeSpanStyles.None, out result);
            var result2 = TimeSpan.TryParseExact(input, formats, formatProvider, TimeSpanStyles.None, out result);
            var result3 = TimeSpan.TryParseExact(input2, format, formatProvider, TimeSpanStyles.None, out result);
            var result4 = TimeSpan.TryParseExact(input2, formats, formatProvider, TimeSpanStyles.None, out result);
        }

        public void RedundantElement(string input, ReadOnlySpan<char> input2, IFormatProvider formatProvider, TimeSpanStyles styles, out TimeSpan result)
        {
            var result11 = TimeSpan.TryParseExact(input, ["c", "t", "T", "g", "g"], formatProvider, out result);
            var result12 = TimeSpan.TryParseExact(input, (string[])["c", "t", "T", "g", "g"], formatProvider, out result);
            var result13 = TimeSpan.TryParseExact(input, new[] { "c", "t", "T", "g", "g" }, formatProvider, out result);
            var result14 = TimeSpan.TryParseExact(input, new string[] { "c", "t", "T", "g", "g" }, formatProvider, out result);

            var result21 = TimeSpan.TryParseExact(input, ["c", "t", "T", "g", "g"], formatProvider, styles, out result);
            var result22 = TimeSpan.TryParseExact(input, (string[])["c", "t", "T", "g", "g"], formatProvider, styles, out result);
            var result23 = TimeSpan.TryParseExact(input, new[] { "c", "t", "T", "g", "g" }, formatProvider, styles, out result);
            var result24 = TimeSpan.TryParseExact(input, new string[] { "c", "t", "T", "g", "g" }, formatProvider, styles, out result);

            var result31 = TimeSpan.TryParseExact(input2, ["c", "t", "T", "g", "g"], formatProvider, out result);
            var result32 = TimeSpan.TryParseExact(input2, (string[])["c", "t", "T", "g", "g"], formatProvider, out result);
            var result33 = TimeSpan.TryParseExact(input2, new[] { "c", "t", "T", "g", "g" }, formatProvider, out result);
            var result34 = TimeSpan.TryParseExact(input2, new string[] { "c", "t", "T", "g", "g" }, formatProvider, out result);

            var result41 = TimeSpan.TryParseExact(input2, ["c", "t", "T", "g", "g"], formatProvider, styles, out result);
            var result42 = TimeSpan.TryParseExact(input2, (string[])["c", "t", "T", "g", "g"], formatProvider, styles, out result);
            var result43 = TimeSpan.TryParseExact(input2, new[] { "c", "t", "T", "g", "g" }, formatProvider, styles, out result);
            var result44 = TimeSpan.TryParseExact(input2, new string[] { "c", "t", "T", "g", "g" }, formatProvider, styles, out result);
        }

        public void NoDetection(string input, ReadOnlySpan<char> input2, string format, string[] formats, IFormatProvider formatProvider, TimeSpanStyles styles, out TimeSpan result)
        {
            var result11 = TimeSpan.TryParseExact(input, "c", null, out result);
            var result12 = TimeSpan.TryParseExact(input, "t", null, out result);
            var result13 = TimeSpan.TryParseExact(input, "T", null, out result);

            var result21 = TimeSpan.TryParseExact(input, format, formatProvider, styles, out result);
            var result22 = TimeSpan.TryParseExact(input, format, formatProvider, TimeSpanStyles.AssumeNegative, out result);
            var result23 = TimeSpan.TryParseExact(input, formats, formatProvider, styles, out result);
            var result24 = TimeSpan.TryParseExact(input, formats, formatProvider, TimeSpanStyles.AssumeNegative, out result);

            var result31 = TimeSpan.TryParseExact(input2, format, formatProvider, styles, out result);
            var result32 = TimeSpan.TryParseExact(input2, format, formatProvider, TimeSpanStyles.AssumeNegative, out result);
            var result33 = TimeSpan.TryParseExact(input2, formats, formatProvider, styles, out result);
            var result34 = TimeSpan.TryParseExact(input2, formats, formatProvider, TimeSpanStyles.AssumeNegative, out result);

            var result41 = TimeSpan.TryParseExact(input, "c", null, styles, out result);
            var result42 = TimeSpan.TryParseExact(input, "t", null, styles, out result);
            var result43 = TimeSpan.TryParseExact(input, "T", null, styles, out result);

            var result51 = TimeSpan.TryParseExact(input, "c", null, TimeSpanStyles.AssumeNegative, out result);
            var result52 = TimeSpan.TryParseExact(input, "t", null, TimeSpanStyles.AssumeNegative, out result);
            var result53 = TimeSpan.TryParseExact(input, "T", null, TimeSpanStyles.AssumeNegative, out result);

            var result61 = TimeSpan.TryParseExact(input, [format, format], formatProvider, out result);
            var result62 = TimeSpan.TryParseExact(input, (string[])[format, format], formatProvider, out result);
            var result63 = TimeSpan.TryParseExact(input, new[] { format, format }, formatProvider, out result);
            var result64 = TimeSpan.TryParseExact(input, new string[] { format, format }, formatProvider, out result);

            var result71 = TimeSpan.TryParseExact(input, [format, format], formatProvider, styles, out result);
            var result72 = TimeSpan.TryParseExact(input, (string[])[format, format], formatProvider, styles, out result);
            var result73 = TimeSpan.TryParseExact(input, new[] { format, format }, formatProvider, styles, out result);
            var result74 = TimeSpan.TryParseExact(input, new string[] { format, format }, formatProvider, styles, out result);

            var result81 = TimeSpan.TryParseExact(input, ["c", "g", "G"], formatProvider, out result);
            var result82 = TimeSpan.TryParseExact(input, (string[])["c", "g", "G"], formatProvider, out result);
            var result83 = TimeSpan.TryParseExact(input, new[] { "c", "g", "G" }, formatProvider, out result);
            var result84 = TimeSpan.TryParseExact(input, new string[] { "c", "g", "G" }, formatProvider, out result);

            var result91 = TimeSpan.TryParseExact(input, ["c", "g", "G"], formatProvider, styles, out result);
            var result92 = TimeSpan.TryParseExact(input, (string[])["c", "g", "G"], formatProvider, styles, out result);
            var result93 = TimeSpan.TryParseExact(input, new[] { "c", "g", "G" }, formatProvider, styles, out result);
            var result94 = TimeSpan.TryParseExact(input, new string[] { "c", "g", "G" }, formatProvider, styles, out result);

            var resultS1 = TimeSpan.TryParseExact(input2, ["c", "g", "G"], formatProvider, out result);
            var resultS2 = TimeSpan.TryParseExact(input2, (string[])["c", "g", "G"], formatProvider, out result);
            var resultS3 = TimeSpan.TryParseExact(input2, new[] { "c", "g", "G" }, formatProvider, out result);
            var resultS4 = TimeSpan.TryParseExact(input2, new string[] { "c", "g", "G" }, formatProvider, out result);

            var resultB1 = TimeSpan.TryParseExact(input2, ["c", "g", "G"], formatProvider, styles, out result);
            var resultB2 = TimeSpan.TryParseExact(input2, (string[])["c", "g", "G"], formatProvider, styles, out result);
            var resultB3 = TimeSpan.TryParseExact(input2, new[] { "c", "g", "G" }, formatProvider, styles, out result);
            var resultB4 = TimeSpan.TryParseExact(input2, new string[] { "c", "g", "G" }, formatProvider, styles, out result);
        }
    }
}