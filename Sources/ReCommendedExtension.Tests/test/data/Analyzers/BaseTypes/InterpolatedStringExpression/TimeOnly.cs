using System;

namespace Test
{
    public class InterpolatedStringItems
    {
        public void RedundantFormatSpecifier(TimeOnly value)
        {
            var result = $"{value:t}";
        }

        public void RedundantFormatSpecifier(TimeOnly? value)
        {
            var result = $"{value:t}";
        }

        public void NoDetection(TimeOnly value)
        {
            var result1 = $"{value:T}";
            var result2 = $"{value:o}";
            var result3 = $"{value:O}";
            var result4 = $"{value:r}";
            var result5 = $"{value:R}";
        }

        public void NoDetection(TimeOnly? value)
        {
            var result1 = $"{value:T}";
            var result2 = $"{value:o}";
            var result3 = $"{value:O}";
            var result4 = $"{value:r}";
            var result5 = $"{value:R}";
        }
    }
}