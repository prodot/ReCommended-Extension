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
            var result = $"{(int){caret}value}";
        }
    }
}