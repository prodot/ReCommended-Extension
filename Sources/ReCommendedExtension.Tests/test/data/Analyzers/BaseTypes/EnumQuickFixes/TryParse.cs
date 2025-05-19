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

        public void TryParse(string s, out SampleEnum result)
        {
            var result1 = Enum.TryParse(s, false{caret}, out result);
        }
    }
}