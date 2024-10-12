using System;

namespace Test
{
    public class Strings
    {
        public void IndexOf(string text, char c)
        {
            var result1 = text.IndexOf(c, 0);
            var result2 = text.IndexOf(c, startIndex: 0);
        }
    }
}