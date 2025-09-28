using System;

namespace Test
{
    public class Guids
    {
        public void Parse(string s, ReadOnlySpan<char> s1, IFormatProvider provider)
        {
            var result1 = Guid.Parse(s, provider);
            var result2 = Guid.Parse(s1, provider);
        }

        public void TryParse(string s, ReadOnlySpan<char> s1, IFormatProvider provider, out Guid result)
        {
            var result1 = Guid.TryParse(s, provider, out result);
            var result2 = Guid.TryParse(s1, provider, out result);
        }
    }
}