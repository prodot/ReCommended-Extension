using System;
using System.Globalization;

namespace Test
{
    public class TimeSpans
    {
        public void TryParseExact(string input, string format, IFormatProvider formatProvider, TimeSpanStyles styles, out TimeSpan result)
        {
            var result_ = TimeSpan.TryParseExact(input, formats: [form{caret}at], formatProvider: formatProvider, styles, out result);
        }
    }
}