using System;

namespace Test
{
    public class Arguments
    {
        public void RedundantArgument(string s, ReadOnlySpan<char> s1, IFormatProvider provider, out Guid result)
        {
            var result11 = Guid.Parse(s, provider);
            var result12 = Guid.Parse(s1, provider);

            var result21 = Guid.TryParse(s, provider, out result);
            var result22 = Guid.TryParse(s1, provider, out result);
        }

        public void NoDetection()
        {
            
        }
    }
}