namespace Test
{
    public class InterpolatedStringItems
    {
        public enum SampleEnum
        {
            Red,
            Green,
            Blue,
        }

        public void RedundantFormatSpecifier(SampleEnum value)
        {
            var result1 = $"{value:G}";
            var result2 = $"{value:g}";
        }

        public void RedundantFormatSpecifier(SampleEnum? value)
        {
            var result1 = $"{value:G}";
            var result2 = $"{value:g}";
        }

        public void NoDetection(SampleEnum value)
        {
            var result = $"{value:F}";
        }

        public void NoDetection(SampleEnum? value)
        {
            var result = $"{value:F}";
        }
    }
}