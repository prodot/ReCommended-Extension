using System;

namespace Test
{
    public class FormatStrings
    {
        public void RedundantFormatSpecifier(TimeSpan value)
        {
            var result1 = string.Format("{0:c}", value);
            var result2 = string.Format("{0:t}", value);
            var result3 = string.Format("{0:T}", value);
        }

        public void RedundantFormatSpecifier(TimeSpan? value)
        {
            var result1 = string.Format("{0:c}", value);
            var result2 = string.Format("{0:t}", value);
            var result3 = string.Format("{0:T}", value);
        }

        public void NoDetection(TimeSpan value)
        {
            var result1 = string.Format("{0:g}", value);
            var result2 = string.Format("{0:G}", value);
        }

        public void NoDetection(TimeSpan? value)
        {
            var result1 = string.Format("{0:g}", value);
            var result2 = string.Format("{0:G}", value);
        }
    }
}