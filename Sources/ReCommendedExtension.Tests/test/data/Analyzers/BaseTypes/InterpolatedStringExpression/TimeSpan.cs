using System;

namespace Test
{
    public class InterpolatedStringItems
    {
        public void RedundantFormatSpecifier(TimeSpan value)
        {
            var result1 = $"{value:c}";
            var result2 = $"{value:t}";
            var result3 = $"{value:T}";
        }

        public void RedundantFormatSpecifier(TimeSpan? value)
        {
            var result1 = $"{value:c}";
            var result2 = $"{value:t}";
            var result3 = $"{value:T}";
        }

        public void NoDetection(TimeSpan value)
        {
            var result1 = $"{value:g}";
            var result2 = $"{value:G}";
        }

        public void NoDetection(TimeSpan? value)
        {
            var result1 = $"{value:g}";
            var result2 = $"{value:G}";
        }
    }
}