using System;

namespace Test
{
    public class Strings
    {
        public void RedundantArguments(string text, char[] c, string s)
        {
            var result1 = text.IndexOfAny(c, 0);
            var result2 = text.IndexOfAny(c, startIndex: 0);
        }

        public void NoDetection(string text, char[] c, int startIndex)
        {
            var result1 = text.IndexOfAny(c, 1);
            var result2 = text.IndexOfAny(c, startIndex);
        }
    }
}