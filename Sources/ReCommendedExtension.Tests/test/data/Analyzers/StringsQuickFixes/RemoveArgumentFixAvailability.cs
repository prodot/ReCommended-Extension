using System;

namespace Test
{
    public class Strings
    {
        public void IndexOf(string text, char c, string s)
        {
            var result1 = text.IndexOf(c, 0);
            var result2 = text.IndexOf(c, startIndex: 0);

            var result3 = text.IndexOf(s, 0);
            var result4 = text.IndexOf(s, startIndex: 0);
        }
    }
}