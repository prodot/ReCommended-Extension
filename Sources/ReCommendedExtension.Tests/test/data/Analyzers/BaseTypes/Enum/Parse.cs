using System;

namespace Test
{
    public class Enums
    {
        enum SampleEnum
        {
            Red,
            Green,
            Blue,
        }

        public void RedundantArgument(string value, ReadOnlySpan<char> value1)
        {
            var result11 = Enum.Parse<SampleEnum>(value, false);
            var result12 = Enum.Parse<SampleEnum>(value1, false);

            var result21 = Enum.Parse(typeof(SampleEnum), value, false);
            var result22 = Enum.Parse(typeof(SampleEnum), value1, false);
        }

        public void NoDetection(string value, ReadOnlySpan<char> value1, bool ignoreCase)
        {
            var result11 = Enum.Parse<SampleEnum>(value, ignoreCase);
            var result12 = Enum.Parse<SampleEnum>(value1, ignoreCase);

            var result21 = Enum.Parse(typeof(SampleEnum), value, ignoreCase);
            var result22 = Enum.Parse(typeof(SampleEnum), value1, ignoreCase);
        }
    }
}