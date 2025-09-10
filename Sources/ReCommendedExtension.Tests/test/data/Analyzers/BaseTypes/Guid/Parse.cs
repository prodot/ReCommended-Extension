using System;

namespace Test
{
    public class Guids
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, IFormatProvider provider)
        {
            var result1 = Guid.Parse(s, provider);
            var result2 = Guid.Parse(s1, provider);
        }

        public void NoDetection() { }
    }
}