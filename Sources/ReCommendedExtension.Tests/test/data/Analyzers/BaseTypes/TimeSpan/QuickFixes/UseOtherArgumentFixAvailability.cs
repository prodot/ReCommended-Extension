using System;
using System.Globalization;

namespace Test
{
    public class TimeSpans
    {
        public void ParseExact(string input, string format, IFormatProvider formatProvider, TimeSpanStyles styles)
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

        public void TryParseExact(string input, string format, IFormatProvider formatProvider, TimeSpanStyles styles, out TimeSpan result)
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
    }
}