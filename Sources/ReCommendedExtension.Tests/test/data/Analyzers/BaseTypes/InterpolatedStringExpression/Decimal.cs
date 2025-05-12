namespace Test
{
    public class InterpolatedStringItems
    {
        public void RedundantFormatSpecifier(decimal number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:g}";
        }

        public void RedundantFormatSpecifier(decimal? number)
        {
            var result1 = $"{number:G}";
            var result2 = $"{number:g}";
        }

        public void RedundantFormatPrecisionSpecifier(decimal number)
        {
            var result11 = $"{number:E6}";
            var result12 = $"{number:e6}";
        }

        public void RedundantFormatPrecisionSpecifier(decimal? number)
        {
            var result11 = $"{number:E6}";
            var result12 = $"{number:e6}";
        }

        public void SuspiciousFormatSpecifier(decimal number)
        {
            var result1 = $"{number:R}";
            var result2 = $"{number:R3}";
            var result3 = $"{number:r}";
            var result4 = $"{number:r3}";
        }

        public void SuspiciousFormatSpecifier(decimal? number)
        {
            var result1 = $"{number:R}";
            var result2 = $"{number:R3}";
            var result3 = $"{number:r}";
            var result4 = $"{number:r3}";
        }

        public void NoDetection(decimal number)
        {
            var result1 = $"{number:G0}";
            var result2 = $"{number:g0}";
            var result3 = $"{number:G1}";
            var result4 = $"{number:g1}";
            var result5 = $"{number:E}";
            var result6 = $"{number:E5}";
        }

        public void NoDetection(decimal? number)
        {
            var result1 = $"{number:G0}";
            var result2 = $"{number:g0}";
            var result3 = $"{number:G1}";
            var result4 = $"{number:g1}";
            var result5 = $"{number:E}";
            var result6 = $"{number:E5}";
        }
    }
}