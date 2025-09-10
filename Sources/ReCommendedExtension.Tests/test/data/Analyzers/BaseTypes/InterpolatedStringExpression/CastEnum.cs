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

        public void CastEnums(SampleEnum value)
        {
            var result1 = $"{(int)value}";
            var result2 = $"{(int?)value}";
            var result3 = $"{(long)value}";
            var result4 = $"{(long?)value}";
        }

        public void CastEnums(SampleEnum? value)
        {
            var result1 = $"{(int?)value}";
            var result2 = $"{(long?)value}";
        }

        public void NoDetection(SampleEnum value)
        {
            var result1 = $"{(byte)value}";
            var result2 = $"{(byte?)value}";
        }

        public void NoDetection(SampleEnum? value)
        {
            var result1 = $"{(int)value}";
            var result2 = $"{(byte)value}";
            var result3 = $"{(byte?)value}";
            var result4 = $"{(long)value}";
        }
    }
}