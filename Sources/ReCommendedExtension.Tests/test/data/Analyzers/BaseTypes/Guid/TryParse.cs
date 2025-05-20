using System;

namespace Test
{
    public class Guids
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, IFormatProvider provider, out Guid result)
        {
            var result1 = Guid.TryParse(s, provider, out result);
            var result2 = Guid.TryParse(s1, provider, out result);
        }

        public void NoDetection() { }
    }
}