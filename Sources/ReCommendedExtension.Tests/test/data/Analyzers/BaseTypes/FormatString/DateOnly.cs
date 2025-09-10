using System;

namespace Test
{
    public class FormatStrings
    {
        public void RedundantFormatSpecifier(DateOnly value)
        {
            var result = string.Format("{0:d}", value);
        }

        public void RedundantFormatSpecifier(DateOnly? value)
        {
            var result = string.Format("{0:d}", value);
        }

        public void NoDetection(DateOnly value)
        {
            var result1 = string.Format("{0:D}", value);
            var result2 = string.Format("{0:o}", value);
            var result3 = string.Format("{0:O}", value);
            var result4 = string.Format("{0:r}", value);
            var result5 = string.Format("{0:R}", value);
            var result6 = string.Format("{0:m}", value);
            var result7 = string.Format("{0:M}", value);
            var result8 = string.Format("{0:y}", value);
            var result9 = string.Format("{0:Y}", value);
        }

        public void NoDetection(DateOnly? value)
        {
            var result1 = string.Format("{0:D}", value);
            var result2 = string.Format("{0:o}", value);
            var result3 = string.Format("{0:O}", value);
            var result4 = string.Format("{0:r}", value);
            var result5 = string.Format("{0:R}", value);
            var result6 = string.Format("{0:m}", value);
            var result7 = string.Format("{0:M}", value);
            var result8 = string.Format("{0:y}", value);
            var result9 = string.Format("{0:Y}", value);
        }
    }
}