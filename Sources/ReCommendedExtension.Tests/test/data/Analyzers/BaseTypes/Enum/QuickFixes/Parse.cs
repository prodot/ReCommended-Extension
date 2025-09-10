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

        public void Parse(string s)
        {
            var result = Enum.Parse<SampleEnum>(s, false{caret});
        }
    }
}