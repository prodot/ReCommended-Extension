using System;

namespace Test
{
    public class Strings
    {
        public void RedundantInvocation(string text, IFormatProvider provider)
        {
            var result1 = text.ToString(provider);
            var result2 = text.ToString(null);
        }

        public void NoDetection(string text, int startIndex, int length)
        {
            text.ToString(provider);
            text.ToString(null);
        }
    }
}