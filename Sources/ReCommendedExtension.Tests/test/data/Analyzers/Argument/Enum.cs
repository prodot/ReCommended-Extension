using System;

namespace Test
{
    public class Arguments
    {
        public enum SampleEnum
        {
            Red,
            Green,
            Blue,
        }

        public void RedundantArgument(string s, ReadOnlySpan<char> s1, out SampleEnum result, out object resultObject)
        {
            var result11 = Enum.Parse<SampleEnum>(s, false);
            var result12 = Enum.Parse<SampleEnum>(s1, false);
            var result13 = Enum.Parse(typeof(SampleEnum), s, false);
            var result14 = Enum.Parse(typeof(SampleEnum), s1, false);

            var result21 = Enum.TryParse<SampleEnum>(s, false, out result);
            var result22 = Enum.TryParse<SampleEnum>(s1, false, out result);
            var result23 = Enum.TryParse(typeof(SampleEnum), s, false, out resultObject);
            var result24 = Enum.TryParse(typeof(SampleEnum), s1, false, out resultObject);
        }

        public void NoDetection(string s, ReadOnlySpan<char> s1, bool ignoreCase, out SampleEnum result, out object resultObject)
        {
            var result11 = Enum.Parse<SampleEnum>(s, ignoreCase);
            var result12 = Enum.Parse<SampleEnum>(s1, ignoreCase);
            var result13 = Enum.Parse(typeof(SampleEnum), s, ignoreCase);
            var result14 = Enum.Parse(typeof(SampleEnum), s1, ignoreCase);

            var result21 = Enum.TryParse<SampleEnum>(s, ignoreCase, out result);
            var result22 = Enum.TryParse<SampleEnum>(s1, ignoreCase, out result);
            var result23 = Enum.TryParse(typeof(SampleEnum), s, ignoreCase, out resultObject);
            var result24 = Enum.TryParse(typeof(SampleEnum), s1, ignoreCase, out resultObject);
        }
    }
}