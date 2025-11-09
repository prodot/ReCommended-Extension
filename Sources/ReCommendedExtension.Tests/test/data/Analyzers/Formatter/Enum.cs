namespace Test
{
    public class Formatters
    {
        public enum SampleEnum
        {
            Red,
            Green,
            Blue,
        }

        public void RedundantFormatSpecifier(SampleEnum value)
        {
            var result11 = $"{value:G}";
            var result12 = $"{value:g}";

            var result21 = string.Format("{0:G}", value);
            var result22 = string.Format("{0:g}", value);

            var result31 = value.ToString(null as string);
            var result32 = value.ToString("");
            var result33 = value.ToString("G");
            var result34 = value.ToString("g");
        }

        public void RedundantFormatSpecifier(SampleEnum? value)
        {
            var result11 = $"{value:G}";
            var result12 = $"{value:g}";

            var result21 = string.Format("{0:G}", value);
            var result22 = string.Format("{0:g}", value);
        }

        public void NoDetection(SampleEnum value)
        {
            var result1 = $"{value:F}";

            var result2 = string.Format("{0:F}", value);

            var result3 = value.ToString("F");
        }

        public void NoDetection(SampleEnum? value)
        {
            var result1 = $"{value:F}";

            var result2 = string.Format("{0:F}", value);
        }
    }
}