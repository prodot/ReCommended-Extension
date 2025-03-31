using System;

namespace Test
{
    public class Strings
    {
        public void IndexOfAny(string text, int startIndex, int count)
        {
            var result = text.IndexOfAny(['a', '{caret}a'], startIndex, count);
        }
    }
}