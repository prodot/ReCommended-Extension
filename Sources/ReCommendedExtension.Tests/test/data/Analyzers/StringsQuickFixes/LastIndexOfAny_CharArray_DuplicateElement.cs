using System;

namespace Test
{
    public class Strings
    {
        public void LastIndexOfAny(string text)
        {
            var result = text.LastIndexOfAny(['a', '{caret}a']);
        }
    }
}