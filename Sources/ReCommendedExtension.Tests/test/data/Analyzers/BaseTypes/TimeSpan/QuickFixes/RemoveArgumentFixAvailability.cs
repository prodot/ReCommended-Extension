using System;
using System.Globalization;

namespace Test
{
    public class TimeSpans
    {
        public void _Constructors(int days, int hours, int minutes, int seconds, int milliseconds)
        {
            var result11 = new TimeSpan(0, hours, minutes, seconds);
            var result12 = new TimeSpan(days, hours, minutes, seconds, 0);
            var result13 = new TimeSpan(days, hours, minutes, seconds, milliseconds, 0);

            TimeSpan result21 = new(0, hours, minutes, seconds);
            TimeSpan result22 = new(days, hours, minutes, seconds, 0);
            TimeSpan result23 = new(days, hours, minutes, seconds, milliseconds, 0);
        }

        public void Parse(string input)
        {
            var result = TimeSpan.Parse(input, null);
        }

        public void ParseExact(string input, string format, string[] formats, IFormatProvider formatProvider)
        {
            var result1 = TimeSpan.ParseExact(input, format, formatProvider, TimeSpanStyles.None);
            var result2 = TimeSpan.ParseExact(input, formats, formatProvider, TimeSpanStyles.None);
        }

        public void ToString(TimeSpan timeSpan, string format, IFormatProvider formatProvider)
        {
            var result11 = timeSpan.ToString(null);
            var result12 = timeSpan.ToString("");
            var result13 = timeSpan.ToString("c");
            var result14 = timeSpan.ToString("t");
            var result15 = timeSpan.ToString("T");

            var result21 = timeSpan.ToString(format, null);

            var result31 = timeSpan.ToString(null, formatProvider);
            var result32 = timeSpan.ToString("", formatProvider);
            var result33 = timeSpan.ToString("c", formatProvider);
            var result34 = timeSpan.ToString("t", formatProvider);
            var result35 = timeSpan.ToString("T", formatProvider);
        }

        public void TryParse(string input, ReadOnlySpan<char> input1, out TimeSpan result)
        {
            var result1 = TimeSpan.TryParse(input, null, out result);
            var result2 = TimeSpan.TryParse(input1, null, out result);
        }

        public void TryParseExact(string input, ReadOnlySpan<char> input2, string format, string[] formats, IFormatProvider formatProvider, out TimeSpan result)
        {
            var result1 = TimeSpan.TryParseExact(input, format, formatProvider, TimeSpanStyles.None, out result);
            var result2 = TimeSpan.TryParseExact(input, formats, formatProvider, TimeSpanStyles.None, out result);
            var result3 = TimeSpan.TryParseExact(input2, format, formatProvider, TimeSpanStyles.None, out result);
            var result4 = TimeSpan.TryParseExact(input2, formats, formatProvider, TimeSpanStyles.None, out result);
        }
    }
}