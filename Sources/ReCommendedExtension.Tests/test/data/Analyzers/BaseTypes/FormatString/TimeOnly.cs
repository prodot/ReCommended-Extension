using System;

namespace Test
{
    public class FormatStrings
    {
        public void RedundantFormatSpecifier(TimeOnly value)
        {
            var result = string.Format("{0:t}", value);
        }

        public void RedundantFormatSpecifier(TimeOnly? value)
        {
            var result = string.Format("{0:t}", value);
        }

        public void NoDetection(TimeOnly value)
        {
            var result1 = string.Format("{0:T}", value);
            var result2 = string.Format("{0:o}", value);
            var result3 = string.Format("{0:O}", value);
            var result4 = string.Format("{0:r}", value);
            var result5 = string.Format("{0:R}", value);
        }

        public void NoDetection(TimeOnly? value)
        {
            var result1 = string.Format("{0:T}", value);
            var result2 = string.Format("{0:o}", value);
            var result3 = string.Format("{0:O}", value);
            var result4 = string.Format("{0:r}", value);
            var result5 = string.Format("{0:R}", value);
        }
    }
}