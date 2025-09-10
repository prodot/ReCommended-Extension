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

        public void RedundantArgument(string value, ReadOnlySpan<char> value1, out SampleEnum result, out object resultObject)
        {
            var result11 = Enum.TryParse<SampleEnum>(value, false, out result);
            var result12 = Enum.TryParse<SampleEnum>(value1, false, out result);

            var result21 = Enum.TryParse(typeof(SampleEnum), value, false, out resultObject);
            var result22 = Enum.TryParse(typeof(SampleEnum), value1, false, out resultObject);
        }

        public void NoDetection(string value, ReadOnlySpan<char> value1, bool ignoreCase, out SampleEnum result, out object resultObject)
        {
            var result11 = Enum.TryParse<SampleEnum>(value, ignoreCase, out result);
            var result12 = Enum.TryParse<SampleEnum>(value1, ignoreCase, out result);

            var result21 = Enum.TryParse(typeof(SampleEnum), value, ignoreCase, out resultObject);
            var result22 = Enum.TryParse(typeof(SampleEnum), value1, ignoreCase, out resultObject);
        }
    }
}