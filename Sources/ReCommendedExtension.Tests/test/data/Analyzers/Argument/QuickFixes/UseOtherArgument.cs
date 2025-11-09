using System;

namespace Test
{
    public class Arguments
    {
        public void OtherArgument(string s, string format, IFormatProvider provider)
        {
            var result = TimeSpan.ParseExact(s, [format{caret}], provider);
        }
    }
}