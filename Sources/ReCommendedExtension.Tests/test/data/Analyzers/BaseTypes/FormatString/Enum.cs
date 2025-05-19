namespace Test
{
    public class FormatStrings
    {
        public enum SampleEnum
        {
            Red,
            Green,
            Blue,
        }

        public void RedundantFormatSpecifier(SampleEnum value)
        {
            var result1 = string.Format("{0:G}", value);
            var result2 = string.Format("{0:g}", value);
        }

        public void RedundantFormatSpecifier(SampleEnum? value)
        {
            var result1 = string.Format("{0:G}", value);
            var result2 = string.Format("{0:g}", value);
        }

        public void NoDetection(SampleEnum value)
        {
            var result = string.Format("{0:F}", value);
        }

        public void NoDetection(SampleEnum? value)
        {
            var result = string.Format("{0:F}", value);
        }
    }
}