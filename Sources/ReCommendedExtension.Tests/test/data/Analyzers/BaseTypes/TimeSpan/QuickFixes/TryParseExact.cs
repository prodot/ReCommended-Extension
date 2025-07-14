using System;

namespace Test
{
    public class TimeSpans
    {
        public void TryParseExact(string input, IFormatProvider formatProvider, out TimeSpan result)
        {
            var result_ = TimeSpan.TryParseExact(input, "c", format{caret}Provider, out result);
        }
    }
}