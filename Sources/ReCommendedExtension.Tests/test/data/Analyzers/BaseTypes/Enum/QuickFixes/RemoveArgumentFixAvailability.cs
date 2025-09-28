using System;

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

        public void Parse(string value, ReadOnlySpan<char> value1)
        {
            var result11 = Enum.Parse<SampleEnum>(value, false);
            var result12 = Enum.Parse<SampleEnum>(value1, false);

            var result21 = Enum.Parse(typeof(SampleEnum), value, false);
            var result22 = Enum.Parse(typeof(SampleEnum), value1, false);
        }

        public void TryParse(string value, ReadOnlySpan<char> value1, out SampleEnum result, out object resultObject)
        {
            var result11 = Enum.TryParse<SampleEnum>(value, false, out result);
            var result12 = Enum.TryParse<SampleEnum>(value1, false, out result);

            var result21 = Enum.TryParse(typeof(SampleEnum), value, false, out resultObject);
            var result22 = Enum.TryParse(typeof(SampleEnum), value1, false, out resultObject);
        }
    }
}