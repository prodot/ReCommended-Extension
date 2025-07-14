using System;

namespace Test
{
    public class TimeSpans
    {
        public void RedundantArgument(TimeSpan timeSpan, string format, IFormatProvider formatProvider)
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

        public void NoDetection(TimeSpan timeSpan, string format, IFormatProvider formatProvider)
        {
            var result11 = timeSpan.ToString(format);
            var result12 = timeSpan.ToString(format, formatProvider);
        }
    }
}