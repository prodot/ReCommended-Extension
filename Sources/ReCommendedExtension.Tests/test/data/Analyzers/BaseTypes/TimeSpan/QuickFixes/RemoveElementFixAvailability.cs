using System;
using System.Globalization;

namespace Test
{
    public class TimeSpans
    {
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

        public void TryParseExact(string input, ReadOnlySpan<char> input2, IFormatProvider formatProvider, TimeSpanStyles styles, out TimeSpan result)
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
    }
}