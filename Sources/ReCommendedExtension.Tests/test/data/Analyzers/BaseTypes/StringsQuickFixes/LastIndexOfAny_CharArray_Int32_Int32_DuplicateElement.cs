using System;

namespace Test
{
    public class Strings
    {
        public void LastIndexOfAny(string text, int startIndex, int count)
        {
            var result = text.LastIndexOfAny(['a', '{caret}a'], startIndex, count);
        }
    }
}