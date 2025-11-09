using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string s, IFormatProvider provider)
        {
            var result = TimeSpan.ParseExact(s, ["c", "t{caret}"], provider);
        }
    }
}