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
    }
}