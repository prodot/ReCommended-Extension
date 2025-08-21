using System;

namespace Test
{
    public class InterpolatedStringItems
    {
        public void RedundantFormatSpecifier(DateOnly value)
        {
            var result = $"{value:d}";
        }

        public void RedundantFormatSpecifier(DateOnly? value)
        {
            var result = $"{value:d}";
        }

        public void NoDetection(DateOnly value)
        {
            var result1 = $"{value:D}";
            var result2 = $"{value:o}";
            var result3 = $"{value:O}";
            var result4 = $"{value:r}";
            var result5 = $"{value:R}";
            var result6 = $"{value:m}";
            var result7 = $"{value:M}";
            var result8 = $"{value:y}";
            var result9 = $"{value:Y}";
        }

        public void NoDetection(DateOnly? value)
        {
            var result1 = $"{value:D}";
            var result2 = $"{value:o}";
            var result3 = $"{value:O}";
            var result4 = $"{value:r}";
            var result5 = $"{value:R}";
            var result6 = $"{value:m}";
            var result7 = $"{value:M}";
            var result8 = $"{value:y}";
            var result9 = $"{value:Y}";
        }
    }
}