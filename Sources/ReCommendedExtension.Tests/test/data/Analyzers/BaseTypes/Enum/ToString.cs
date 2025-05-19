namespace Test
{
    public class Enums
    {
        public enum SampleEnum
        {
            Red,
            Green,
            Blue,
        }

        public void RedundantArgument(SampleEnum e)
        {
            var result1 = e.ToString(null as string);
            var result2 = e.ToString("");
            var result3 = e.ToString("G");
            var result4 = e.ToString("g");
        }

        public void NoDetection(SampleEnum e)
        {
            var result = e.ToString("F");
        }
    }
}