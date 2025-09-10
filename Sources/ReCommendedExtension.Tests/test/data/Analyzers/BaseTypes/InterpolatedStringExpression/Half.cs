using System;

namespace Test
{
    public class InterpolatedStringItems
    {
        public void RedundantFormatSpecifier(Half number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
        }

        public void RedundantFormatSpecifier(Half? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:G0}";
        }

        public void RedundantFormatPrecisionSpecifier(Half number)
        {
            var result11 = $"{number:E6}";
            var result12 = $"{number:e6}";

            var result21 = $"{number:R3}";
            var result22 = $"{number:r3}";
        }

        public void RedundantFormatPrecisionSpecifier(Half? number)
        {
            var result11 = $"{number:E6}";
            var result12 = $"{number:e6}";

            var result21 = $"{number:R3}";
            var result22 = $"{number:r3}";
        }

        public void NoDetection(Half number)
        {
            var result1 = $"{number:G1}";
            var result2 = $"{number:g}";
            var result3 = $"{number:g0}";
            var result4 = $"{number:g1}";
            var result5 = $"{number:E}";
            var result6 = $"{number:E5}";
            var result7 = $"{number:R}";
            var result8 = $"{number:r}";
        }

        public void NoDetection(Half? number)
        {
            var result1 = $"{number:G1}";
            var result2 = $"{number:g}";
            var result3 = $"{number:g0}";
            var result4 = $"{number:g1}";
            var result5 = $"{number:E}";
            var result6 = $"{number:E5}";
            var result7 = $"{number:R}";
            var result8 = $"{number:r}";
        }
    }
}