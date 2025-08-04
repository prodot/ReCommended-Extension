using System;
using System.Globalization;

namespace Test
{
    public class DateTimes
    {
        public void ParseExact(string input, ReadOnlySpan<char> input1, IFormatProvider formatProvider, DateTimeStyles styles)
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
    }
}