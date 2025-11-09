using System;

namespace Test
{
    public class Formatters
    {
        public void RedundantFormatSpecifier(TimeSpan value)
        {
            var result11 = $"{value:c}";
            var result12 = $"{value:t}";
            var result13 = $"{value:T}";

            var result21 = string.Format("{0:c}", value);
            var result22 = string.Format("{0:t}", value);
            var result23 = string.Format("{0:T}", value);
        }

        public void RedundantFormatSpecifier(TimeSpan? value)
        {
            var result11 = $"{value:c}";
            var result12 = $"{value:t}";
            var result13 = $"{value:T}";

            var result21 = string.Format("{0:c}", value);
            var result22 = string.Format("{0:t}", value);
            var result23 = string.Format("{0:T}", value);
        }

        public void RedundantFormatSpecifierProvider(TimeSpan timeSpan, string format, IFormatProvider formatProvider)
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

        public void NoDetection(TimeSpan value)
        {
            var result11 = $"{value:g}";
            var result12 = $"{value:G}";

            var result21 = string.Format("{0:g}", value);
            var result22 = string.Format("{0:G}", value);
        }

        public void NoDetection(TimeSpan? value)
        {
            var result11 = $"{value:g}";
            var result12 = $"{value:G}";

            var result21 = string.Format("{0:g}", value);
            var result22 = string.Format("{0:G}", value);
        }

        public void NoDetection(TimeSpan timeSpan, string format, IFormatProvider formatProvider)
        {
            var result11 = timeSpan.ToString(format);
            var result12 = timeSpan.ToString(format, formatProvider);
        }
    }
}