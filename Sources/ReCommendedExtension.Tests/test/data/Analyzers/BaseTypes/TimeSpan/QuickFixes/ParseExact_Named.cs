using System;
using System.Globalization;

namespace Test
{
    public class TimeSpans
    {
        public void ParseExact(string input, IFormatProvider formatProvider, TimeSpanStyles styles)
        {
            var result = TimeSpan.ParseExact(input, "c", formatProvider: format{caret}Provider, styles);
        }
    }
}