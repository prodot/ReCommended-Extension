namespace Test
{
    public class FormatStrings
    {
        public void RedundantFormatSpecifier(double number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
        }

        public void RedundantFormatSpecifier(double? number)
        {
            var result1 = string.Format("{0:G}", number);
            var result2 = string.Format("{0:G0}", number);
        }

        public void RedundantFormatPrecisionSpecifier(double number)
        {
            var result11 = string.Format("{0:E6}", number);
            var result12 = string.Format("{0:e6}", number);
        }

        public void RedundantFormatPrecisionSpecifier(double? number)
        {
            var result11 = string.Format("{0:E6}", number);
            var result12 = string.Format("{0:e6}", number);
        }

        public void PassOtherFormatSpecifier(double number)
        {
            var result1 = string.Format("{0:R}", number);
            var result2 = string.Format("{0:R3}", number);
            var result3 = string.Format("{0:r}", number);
            var result4 = string.Format("{0:r3}", number);
        }

        public void PassOtherFormatSpecifier(double? number)
        {
            var result1 = string.Format("{0:R}", number);
            var result2 = string.Format("{0:R3}", number);
            var result3 = string.Format("{0:r}", number);
            var result4 = string.Format("{0:r3}", number);
        }

        public void NoDetection(double number)
        {
            var result1 = string.Format("{0:G1}", number);
            var result2 = string.Format("{0:g}", number);
            var result3 = string.Format("{0:g0}", number);
            var result4 = string.Format("{0:g1}", number);
            var result5 = string.Format("{0:E}", number);
            var result6 = string.Format("{0:E5}", number);
        }

        public void NoDetection(double? number)
        {
            var result1 = string.Format("{0:G1}", number);
            var result2 = string.Format("{0:g}", number);
            var result3 = string.Format("{0:g0}", number);
            var result4 = string.Format("{0:g1}", number);
            var result5 = string.Format("{0:E}", number);
            var result6 = string.Format("{0:E5}", number);
        }
    }
}